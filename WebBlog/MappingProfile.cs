using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Request.Comment;
using WebBlog.Contracts.Models.Request.Role;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Role;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;

namespace WebBlog
{
    /// <summary>
    /// Настройки маппинга всех сущностей приложения
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// В конструкторе настроим соответствие сущностей при маппинге
        /// </summary>
        public MappingProfile()
        {
            CreateMap<EditArticleRequest,Article >();
            CreateMap<NewArticleRequest, Article>();
            CreateMap<Article, EditArticleRequest>();

            
            CreateMap<UserViewModel, BlogUser>();
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
            CreateMap<BlogUser, UserViewModel>()
                .ForMember(dest => dest.Roles, opt => opt
                .MapFrom(src => src.Roles.Select(x => new RoleSelectViewModel() { Id = x.Id, Name = x.Name }).ToList()));
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.



            CreateMap<CommentViewModel, Comment>();
            CreateMap<TagViewModel,Tag >();

            CreateMap<NewCommentRequest, Comment>();
            CreateMap<EditCommentRequest, Comment>();

            CreateMap<TagRequest, Tag>();

            CreateMap<NewUserRequest,BlogUser>();
            CreateMap<Comment, CommentViewModel>();
            CreateMap<Tag, TagViewModel>()
                .ForMember(dest => dest.ArticleCount, opt=>opt.MapFrom(src=>src.Articles.Count));
            CreateMap<Tag, EditTagRequest>();


        
            //CreateMap<Article, NewArticleRequest>();
            CreateMap<Article, ArticleViewModel>()
                .ForMember(dest=> dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName));

            CreateMap<EditTagRequest, Tag>();
            CreateMap<NewTagRequest, Tag>();


            CreateMap<BlogRole, RoleViewModel>();
            CreateMap<BlogRole, EditRoleRequest>();
            CreateMap<NewRoleRequest, BlogRole>();
            CreateMap<EditRoleRequest, BlogRole>();

           

            

        }
    }
}
