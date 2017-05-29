using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Entities;
using BLL.Interfaces;
using BLL.Infrastructure;
using BLL.DTO;
using MathNet.Numerics.LinearAlgebra;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;


namespace BLL.Services
{
    public class BlogService: IBlogService
    {
        IUnitOfWork Database { get; set; }

        public BlogService(IUnitOfWork uow)
        {
            Database = uow;
        }

        //additional options (TF-IDF)
        double TF(string word, string document)
        {
            string[] words_in_document = document.Split(new char[] { ' ', ',', '.', ':', ';', '\t', '@', '#', '-' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words_in_document.Count(); i++)
            {
                words_in_document[i] = words_in_document[i].ToLower();
            }
            int word_amount = 0;
            foreach (var element in words_in_document)
            {
                if (word == element)
                {
                    word_amount++;
                }
            }
            double TF = (double)word_amount / words_in_document.Length;
            return TF;
        }

        double IDF(string word, List<string> list_of_docs)
        {
            int documents_withword_amount = 0;
            foreach (var doc in list_of_docs)
            {
                string[] words_in_document = doc.Split(new char[] { ' ', ',', '.', ':', ';', '\t', '@', '#', '-' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words_in_document.Length; i++)
                {
                    words_in_document[i] = words_in_document[i].ToLower();
                }
                foreach (var element in words_in_document)
                {
                    if (word == element)
                    {
                        documents_withword_amount++;
                        break;
                    }
                }
            }
            double IDF = Math.Log((double)list_of_docs.Count() / documents_withword_amount);
            return IDF;
        }

        double[,] TF_IDF_Normalization(List<string> list_of_words, List<string> list_of_docs)
        {
            double[,] matrix_result = new double[list_of_words.Count, list_of_docs.Count];
            int row = 0;
            foreach (var word in list_of_words)
            {
                int col = 0;
                foreach (var doc in list_of_docs)
                {
                    matrix_result[row, col] = TF(word, doc) * IDF(word, list_of_docs);
                    col++;
                }
                row++;
            }
            return matrix_result;
        }
        //

        public void Create(BlogDTO blogDto)
        {
            Blog blog = Database.Blogs.Get(blogDto.Id);
            if (blog == null)
            {
                blog = new Blog();
                blog.BlogId = blogDto.Id;
                blog.BlogName = blogDto.BlogName;
                blog.BlogDescription = blogDto.blogDescription;
                blog.ApplicationUser = Database.UserManager.Users.First(x=>x.Id== blogDto.UserId);
                blog.CategoryID = blogDto.CategoryId;
                blog.date_of_creation = blogDto.date_of_creation;
                blog.date_of_last_update = blogDto.date_of_last_update;
                Database.Blogs.Create(blog);
                Database.Save();
            }
        }

        public void Delete(int blog_id)
        {
            Blog blog = Database.Blogs.GetAll().Where(x => x.BlogId == blog_id).FirstOrDefault();
            List<Article> articles_for_blog = Database.Articles.GetAll().Where(x=>x.Blog.BlogId==blog_id).ToList();
            List<int> articlesIds = new List<int>();
            foreach(var article in articles_for_blog)
            {
                articlesIds.Add(article.Id);
            }
            foreach (var article_id in articlesIds)
            {
                IArticleService articleService = new ArticleService(Database);
                articleService.Delete(article_id);
            }
            Database.Blogs.Delete(blog_id);
            Database.Save();
        }

        public IEnumerable<BlogDTO> GetAllForUser(string userId)
        {
            ICollection<Blog> blogs = new List<Blog>();
            var all_blogs = Database.Blogs.GetAll().Where(x => x.ApplicationUserId == userId);
            if (all_blogs.Count()!=0)
            {
                foreach (var b in all_blogs)
                {
                    blogs.Add(b);
                }
            }
            List<BlogDTO> blogsDTO = new List<BlogDTO>();
            foreach(var blog in blogs)
            {
                BlogDTO blogDTO = new BlogDTO { Id = blog.BlogId, CategoryId = (int)blog.CategoryID, date_of_creation = blog.date_of_creation, date_of_last_update = blog.date_of_last_update, UserId = blog.ApplicationUserId, BlogName = blog.BlogName, blogDescription=blog.BlogDescription};
                blogsDTO.Add(blogDTO);
            }
            return blogsDTO;
        }

        public IEnumerable<BlogDTO> GetAll()
        {
            ICollection<Blog> blogs = new List<Blog>();
            var all_blogs = Database.Blogs.GetAll();
            if (all_blogs.Count() != 0)
            {
                foreach (var b in all_blogs)
                {
                    blogs.Add(b);
                }
            }
            List<BlogDTO> blogsDTO = new List<BlogDTO>();
            foreach (var blog in blogs)
            {
                BlogDTO blogDTO = new BlogDTO { Id = blog.BlogId, CategoryId = (int)blog.CategoryID, date_of_creation = blog.date_of_creation, date_of_last_update = blog.date_of_last_update, UserId = blog.ApplicationUserId, BlogName = blog.BlogName, blogDescription=blog.BlogDescription };
                blogsDTO.Add(blogDTO);
            }
            return blogsDTO;
        }

        public CategoryDTO GetCategoryForBlog(int BlogId)
        {
            int CategoryId = (int)Database.Blogs.Get(BlogId).CategoryID;
            Category Category = (Category)Database.Categories.Get(CategoryId);
            CategoryDTO CategoryDTO = new CategoryDTO { CategoryId = Category.CategoryId, category_name = Category.category_name };
            return CategoryDTO;
        }

        public string GetUserNicknameForBlog(int BlogId)
        {
            string nickname = "";
            string user_id = Database.Blogs.Get(BlogId).ApplicationUser.Id;
            var user = Database.ClientManager.FindById(user_id);
            nickname = user.nickname;
            return nickname;
        }

        public string GetUserIdForBlog(int BlogId)
        {
            string user_id = Database.Blogs.Get(BlogId).ApplicationUser.Id;
            return user_id;
        }

        public ICollection<CategoryDTO> GetAllCategories()
        {
            IEnumerable<Category> Categories = Database.Categories.GetAll();
            List<CategoryDTO> CategoriesDTO = new List<CategoryDTO>();
            foreach(var cat in Categories)
            {
                CategoryDTO catDTO = new CategoryDTO { CategoryId = cat.CategoryId, category_name = cat.category_name };
                CategoriesDTO.Add(catDTO);
            }
            return CategoriesDTO;
        }

        public ICollection<ArticleDTO> GetArticlesForBlog(int BlogId)
        {
            var articles = Database.Articles.GetAll().Where(x=>x.Blog.BlogId==BlogId);
            ICollection<ArticleDTO> articlesDTO = new List<ArticleDTO>();
            foreach(var a in articles)
            {
                ArticleDTO art_DTO = new ArticleDTO();
                art_DTO.ArticleId = a.Id;
                art_DTO.title = a.title;
                art_DTO.text = a.text;
                art_DTO.Status_of_ArticleId = a.Status_of_Article.Status_of_ArticleId;
                art_DTO.Comments = new List<CommentDTO>();
                foreach (var comment in a.Comments)
                {
                    CommentDTO commentDTO = new CommentDTO();
                    commentDTO.article_id = comment.Article.Id;
                    commentDTO.comment_id = comment.CommentId;
                    commentDTO.datetime = comment.date_of_comment;
                    commentDTO.image_path = comment.ApplicationUser.ClientProfile.avatar_path;
                    commentDTO.status_id = comment.Status_of_Comment.Status_of_CommentId;
                    commentDTO.text = comment.text;
                    commentDTO.user_name = comment.ApplicationUser.Email;
                    art_DTO.Comments.Add(commentDTO);
                }
                art_DTO.ImagePaths = new List<string>();
                foreach(var img in a.Images)
                {
                    art_DTO.ImagePaths.Add(img.image_path);
                }
                articlesDTO.Add(art_DTO);
            }
            return articlesDTO;
        }

        public UserDTO GetUserById(string id)
        {
            IUserService userService = new UserService(Database);
            return userService.FindById(id);
        }

        public List<string> Recommended_materials(List<string> list_of_docs, string UserName)
        {
            List<int> recommendedArticlesIds = new List<int>();
            List<string> tags = new List<string>();
            var blogs = Database.Blogs.Find(x=>x.ApplicationUser.Email== UserName);
            foreach(var blog in blogs)
            {
                var articles = Database.Articles.GetAll().Where(x => x.BlogId == blog.BlogId);
                foreach(var article in articles)
                {
                    var _tags = Database.Tags.GetAll().Where(x => x.Articles.Contains(article));
                    foreach(var _tag in _tags)
                    {
                        if (!tags.Contains(_tag.name))
                        {
                            tags.Add(_tag.name);
                        }
                    }
                }
            }
            int n = list_of_docs.Count();//number of documents
            int m = 0;//number of words
            List<string> all_words = new List<string>();
            foreach (var doc in list_of_docs)
            {
                foreach (var word in doc.Split(new char[] { ' ', ',', '.', ':', ';', '\t', '@', '#', '-' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!all_words.Contains(word.ToLower()))
                    {
                        all_words.Add(word.ToLower());
                    }
                }
            }
            List<string> all_words_lower = new List<string>();
            foreach (var element in all_words)
            {
                all_words_lower.Add((string)element.ToLower());
            }
            List<string> stop_words = new List<string>();
            using (StreamReader sr = new StreamReader("stopwords.txt"))
            {
                stop_words = sr.ReadToEnd().Split(new char[] { ' ', ',', '.', ':', ';', '\t', '@', '#', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> remained_words = new List<string>();
            foreach (var element in all_words_lower)
            {
                if (!stop_words.Contains(element) && Int32.TryParse(element, out int a) == false)
                {
                    remained_words.Add(element);
                }
            }
            //=====================================================
            m = remained_words.Count();
            double[,] matrix_wordIndoc = new double[m, n];
            int i = 0;
            int j = 0;
            List<double> coeff_of_notUnique = new List<double>();
            foreach (var element in remained_words)
            {
                j = 0;
                int doc_count_for_element = 0;
                foreach (var document in list_of_docs)
                {
                    bool is_in_doc = false;
                    foreach (var word in document.Split(new char[] { ' ', ',', '.', ':', ';', '\t', '@', '#', '"', ')', '(' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (word.ToLower() == element)
                        {
                            matrix_wordIndoc[i, j] += 1;
                            is_in_doc = true;
                        }
                    }
                    if (is_in_doc == true)
                    {
                        doc_count_for_element++;
                    }
                    j++;
                }
                if (doc_count_for_element > 1)
                {
                    coeff_of_notUnique.Add(i);
                }
                i++;
            }
            //============================================
            List<string> listof_unique_words = new List<string>();
            foreach (var index in coeff_of_notUnique)
            {
                listof_unique_words.Add(remained_words.ElementAt((int)index));
            }
            double[,] matrix_ofNotUnique = new double[coeff_of_notUnique.Count, n];
            int rows = 0;
            for (int i1 = 0; i1 < matrix_wordIndoc.GetLength(0); i1++)
            {
                if (coeff_of_notUnique.Contains(i1))
                {
                    for (int j1 = 0; j1 < matrix_wordIndoc.GetLength(1); j1++)
                    {
                        matrix_ofNotUnique[rows, j1] = matrix_wordIndoc[i1, j1];
                    }
                    rows++;
                }
            }
           
            /////////////////////////////////

            var normalized_matrix = TF_IDF_Normalization(listof_unique_words, list_of_docs);
            double[,] matrix_ofNotUnique_TFIDF = new double[normalized_matrix.GetLength(0), normalized_matrix.GetLength(1)];
            for (int i1 = 0; i1 < normalized_matrix.GetLength(0); i1++)
            {
                for (int j1 = 0; j1 < normalized_matrix.GetLength(1); j1++)
                {
                    matrix_ofNotUnique_TFIDF[i1, j1] = normalized_matrix[i1, j1];
                }
            }

            /////////////////////////////////
            m = matrix_ofNotUnique.GetLength(0);
            double[] W = new double[n];
            double[,] U = new double[m, Math.Min(m, n)];
            double[,] Vt = new double[Math.Min(m, n), n];
            alglib.svd.rmatrixsvd(matrix_ofNotUnique, m, n, 1, 1, 2, ref W, ref U, ref Vt);

            int principal_amount = GetAllForUser(GetUserByName(UserName).Id).Count();
            Matrix<double> U_firtsprincipal_amountcols = Matrix<double>.Build.Sparse(m, principal_amount);
            for (int i1 = 0; i1 < U.GetLength(0); i1++)
            {
                for (int j1 = 0; j1 < principal_amount; j1++)
                {
                    U_firtsprincipal_amountcols[i1, j1] = U[i1, j1];
                }
            }
            Matrix<double> W_firtsprincipal_amount = Matrix<double>.Build.Sparse(principal_amount, principal_amount);
            for (int i1 = 0; i1 < principal_amount; i1++)
            {
                for (int j1 = 0; j1 < principal_amount; j1++)
                {
                    if (i1 == j1)
                    {
                        W_firtsprincipal_amount[i1, j1] = W[i1];
                    }
                }
            }
            Matrix<double> Vt_firtsprincipal_amountrows = Matrix<double>.Build.Sparse(principal_amount, n);
            for (int i1 = 0; i1 < principal_amount; i1++)
            {
                for (int j1 = 0; j1 < Vt.GetLength(1); j1++)
                {
                    Vt_firtsprincipal_amountrows[i1, j1] = Vt[i1, j1];
                }
            }

            double[,] distances_words = new double[listof_unique_words.Count, listof_unique_words.Count];
            int row = 0;
            foreach (var word1 in listof_unique_words)
            {
                int col = 0;
                foreach (var word2 in listof_unique_words)
                {
                    double top = 0;
                    double bottom_1 = 0;
                    double bottom_2 = 0;
                    for (int i1 = 0; i1 < principal_amount; i1++)
                    {
                        for (int j1 = 0; j1 < principal_amount; j1++)
                        {
                            top += W_firtsprincipal_amount[i1, j1] * U_firtsprincipal_amountcols[row, i1] * U_firtsprincipal_amountcols[col, j1];
                            bottom_1 += W_firtsprincipal_amount[i1, j1] * U_firtsprincipal_amountcols[row, i1] * U_firtsprincipal_amountcols[row, j1];
                            bottom_2 += W_firtsprincipal_amount[i1, j1] * U_firtsprincipal_amountcols[col, i1] * U_firtsprincipal_amountcols[col, j1];
                        }
                    }
                    bottom_1 = Math.Sqrt(bottom_1);
                    bottom_2 = Math.Sqrt(bottom_2);
                    distances_words[row, col] = top / (bottom_1 * bottom_2);
                    col++;
                }
                row++;
            }
            
            int info = 0;
            double[,] matrix_res = new double[distances_words.GetLength(0), principal_amount];
            int[] points = new int[distances_words.GetLength(1)];
            alglib.kmeansgenerate(distances_words, distances_words.GetLength(0), distances_words.GetLength(1), principal_amount, 5, out info, out matrix_res, out points);

            /////////////////////////////////////////////////////////

            List<string>[] clusters = new List<string>[principal_amount];
            for (int i1 = 0; i1 < principal_amount; i1++)
            {
                clusters[i1] = new List<string>();
            }
            int counter = 0;
            foreach (var word in listof_unique_words)
            {
                clusters[points[counter]].Add(word);
                counter++;
            }
            foreach(var cluster in clusters)
            {
                foreach(var element in cluster)
                {
                    if(!tags.Contains("#"+element))
                    {
                        tags.Add("#"+element);
                    }
                }
            }
            return tags;
        }

        public ICollection<ArticleDTO> GetArticlesWithTags(string tags, List<int?> BlogIds_notconsidered = null, int BlogId = 0)
        {
            string[] all_tags = tags.Split(' ');
            List<ArticleDTO> articles_with_tagsDTO = new List<ArticleDTO>();
            List<Article> articles_with_tags = new List<Article>();
            List<Article> articles_considered = new List<Article>();
            if (BlogId==0)
            {
                if (BlogIds_notconsidered != null)
                {
                    var all_articles = Database.Articles.GetAll().ToList();
                    foreach (var article in all_articles)
                    {
                        if (!BlogIds_notconsidered.Contains(article.BlogId))
                        {
                            articles_considered.Add(article);
                        }
                    }
                }
            }
            else
            {
                articles_considered = Database.Articles.GetAll().Where(x => x.Blog.BlogId == BlogId).ToList();
            }
            foreach(var article in articles_considered)
            {
                foreach(var tag in all_tags)
                {
                    Tag currentTag = Database.Tags.GetAll().Where(x=>x.name==tag).FirstOrDefault();
                    if (currentTag != null)
                    {
                        if (article.Tags.Contains(currentTag))
                        {
                            articles_with_tags.Add(article);
                            break;
                        }
                    }
                }
            }
            foreach(var article_with_tags in articles_with_tags)
            {
                ArticleDTO art_DTO = new ArticleDTO();
                art_DTO.ArticleId = article_with_tags.Id;
                art_DTO.BlogId = article_with_tags.Blog.BlogId;
                art_DTO.title = article_with_tags.title;
                art_DTO.text = article_with_tags.text;
                art_DTO.Status_of_ArticleId = article_with_tags.Status_of_Article.Status_of_ArticleId;
                art_DTO.Comments = new List<CommentDTO>();
                foreach (var comment in article_with_tags.Comments)
                {
                    CommentDTO commentDTO = new CommentDTO();
                    commentDTO.article_id = comment.Article.Id;
                    commentDTO.comment_id = comment.CommentId;
                    commentDTO.datetime = comment.date_of_comment;
                    commentDTO.image_path = comment.ApplicationUser.ClientProfile.avatar_path;
                    commentDTO.status_id = comment.Status_of_Comment.Status_of_CommentId;
                    commentDTO.text = comment.text;
                    commentDTO.user_name = comment.ApplicationUser.Email;
                    art_DTO.Comments.Add(commentDTO);
                }
                art_DTO.ImagePaths = new List<string>();
                foreach (var img in article_with_tags.Images)
                {
                    art_DTO.ImagePaths.Add(img.image_path);
                }
                articles_with_tagsDTO.Add(art_DTO);
            }
            return articles_with_tagsDTO;
        }

        public UserDTO GetUserByName(string name)
        {
            IUserService userService = new UserService(Database);
            UserDTO user = userService.GetAll().Where(x => x.Email == name).FirstOrDefault();
            return user;
        }

        public void EditDescription(BlogDTO blog)
        {
            Blog blog_to_edit = Database.Blogs.GetAll().Where(x=>x.BlogId==blog.Id).FirstOrDefault();
            blog_to_edit.BlogDescription = blog.blogDescription;
            blog_to_edit.BlogName = blog.BlogName;
            blog_to_edit.Category = Database.Categories.Get(blog.CategoryId);
            Database.Blogs.Update(blog_to_edit);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
