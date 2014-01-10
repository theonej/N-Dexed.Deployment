using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using N_Dexed.Deployment.Common.Domain.Blog;
using System.Collections.Generic;
using N_Dexed.Deployment.AWS.Repositories;

namespace N_Dexed.Deployment.Tests.AWS.Repositories
{
    [TestClass]
    public class DynamoBlogRepository_Test
    {
        [TestMethod]
        public void CreateBlogEntryThenSearchForItThenDeleteIt()
        {
            BlogInfo blog = new BlogInfo();
            blog.Id = Guid.NewGuid();
            blog.BlogDate = DateTime.Now;
            blog.Body = "Test Content For a Blog Entry";
            blog.Tags = new List<string>();
            blog.Tags.Add("test");
            blog.Tags.Add("blog entry");
            blog.Title = "Test Blog Entry";

            DynamoBlogRepository repository = new DynamoBlogRepository();

            try
            {
                repository.Save(blog);

                BlogInfo foundBlog = repository.Search(blog).First();
                Assert.AreEqual(foundBlog.Id, blog.Id);
                Assert.AreEqual(foundBlog.BlogDate, blog.BlogDate);
                Assert.AreEqual(foundBlog.Body, blog.Body);
                Assert.AreEqual(foundBlog.Tags.Count, blog.Tags.Count);
            }
            finally
            {
                repository.Delete(blog);
            }
        }
    }
}
