using AssistantTrainingCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssistantTrainingCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<GroupWorker> GroupWorkers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingName> TrainingNames { get; set; }
        public DbSet<InstructionGroup> InstructionGroups { get; set; }
        public DbSet<TrainingGroup> TrainingGroups { get; set; }
    }
}