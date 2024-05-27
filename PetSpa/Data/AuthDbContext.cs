using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PetSpa.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerRoleId = "f6920e9c-eefe-4e77-bef2-19faa11fdec4";
            var writerRoleId = "caa3f555-e45f-480d-a8cb-0560a8c51b7d";
            
            var roles = new List<IdentityRole> {



               
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                 new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
            //        Id: Định danh duy nhất cho vai trò, được lấy từ các biến writerRoleId và readerRoleId.
            //ConcurrencyStamp: Concurrency token để đảm bảo tính đồng bộ khi cập nhật.
            //Name: Tên của vai trò.
            //NormalizedName: Tên của vai trò được chuẩn hóa, thường là viết hoa.
        }
    }
}
