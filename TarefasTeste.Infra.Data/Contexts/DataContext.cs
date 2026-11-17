using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Infra.Data.Mappings;

namespace TarefasTeste.Infra.Data.Contexts
{
    public class DataContext : DbContext
    {
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost,1434;Initial Catalog=master;User ID=sa;Password=Coti@2025;Encrypt=False");
        }


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TarefaMap());
        }
    }
}
