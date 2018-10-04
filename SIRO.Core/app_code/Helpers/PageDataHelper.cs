using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace SIRO.Core.app_code.Helpers
{
    internal static class PageDataHelper
    {
        public static void GetChildrenRecursive(ContentReference parent, List<PageData> list)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            foreach (var child in repository.GetChildren<PageData>(parent))
            {
                list.Add(child);
                GetChildrenRecursive(child.ContentLink, list);
            }
        }

        public static IEnumerable<T> GetDescendants<T>(this PageData page)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var contentType = repository.Load(typeof(T));
            int? pageTypeID = contentType.ID;

            if (pageTypeID.HasValue)
            {
                PropertyCriteria pageTypeCriteria = new PropertyCriteria
                {
                    Condition = CompareCondition.Equal,
                    Name = "PageTypeID",
                    Type = PropertyDataType.PageType,
                    Value = pageTypeID.Value.ToString()
                };
                PropertyCriteriaCollection criterias = new PropertyCriteriaCollection
                {
                    pageTypeCriteria
                };

                var pageCriteriaQueryService = ServiceLocator.Current
                                               .GetInstance<IPageCriteriaQueryService>();
                var descendants = pageCriteriaQueryService
                                  .FindPagesWithCriteria(page.ContentLink.ToPageReference(), criterias).Cast<T>();

                return descendants;
            }
            return null;
        }
    }
}
