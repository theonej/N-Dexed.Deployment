using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Blog;
using N_Dexed.Deployment.Common.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.SystemInstructions.Handlers
{
    public class BlogInstructionHandler : ISystemInstructionHandler<AddBlogEntryInstruction>
    {
        private readonly IRepository<BlogInfo> m_BlogRepository;

        public BlogInstructionHandler(IRepository<BlogInfo> blogRepository)
        {
            Condition.Requires(blogRepository).IsNotNull();

            m_BlogRepository = blogRepository;
        }

        public void Handle(AddBlogEntryInstruction instruction)
        {
            BlogInfo blog = new BlogInfo();
            blog.Id = instruction.Id;
            blog.BlogDate = DateTime.Now;
            blog.Body = instruction.Body;
            blog.Tags = instruction.Tags;
            blog.Title = instruction.Title;

            m_BlogRepository.Save(blog);
        }
    }
}
