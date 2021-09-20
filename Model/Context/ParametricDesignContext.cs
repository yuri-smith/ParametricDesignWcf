using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ParametricDesignWcfServiceLibrary.Model;

namespace ParametricDesignWcfServiceLibrary.Model.Context
{
    public class ParametricDesignContext : DbContext
    {
        public ParametricDesignContext()
            : base("ParametricDesignWcfServiceLibrary.Properties.Settings.ConnectToParametricDesign")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        static ParametricDesignContext()
        {
            Database.SetInitializer<ParametricDesignContext>(new DataContextInitializer());
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<TypeProduct> TypeProducts { get; set; }
        public DbSet<Combination> Combinations { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<TypeProductParameter> TypeProductParameters { get; set; }
        public DbSet<CombinationParameter> CombinationParameters { get; set; }
        public DbSet<Fitting> Fittings { get; set; }
        public DbSet<CombinationFitting> CombinationFittings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SellerCustomerCompany> SellerCustomerCompanies { get; set; }
        public DbSet<Dim> Dims { get; set; }
        public DbSet<NodeDetailFitting> NodeDetailFittings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        //public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeProductParameter>()
            .HasKey(d => new { d.TypeProductTypeProductID, d.ParameterParameterID });
            modelBuilder.Entity<CombinationParameter>()
            .HasKey(d => new { d.CombinationCombinationID, d.ParameterParameterID });
            modelBuilder.Entity<SellerCustomerCompany>()
            .HasKey(d => new { d.SellerCompanyID, d.CustomerCompanyID });
            modelBuilder.Entity<NodeDetailFitting>()
            .HasKey(d => new { d.NodeFittingID, d.DetailFittingID });

            modelBuilder.Entity<SellerCustomerCompany>()
                .HasRequired(x => x.Seller)
                .WithMany(x => x.Sellers)
                .HasForeignKey(x => x.SellerCompanyID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SellerCustomerCompany>()
                .HasRequired(x => x.Customer)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.CustomerCompanyID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Order>()
                .HasRequired(x => x.Seller)
                .WithMany(x => x.AsSellerOrders)
                .HasForeignKey(x => x.SellerCompanyID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasRequired(x => x.Customer)
                .WithMany(x => x.AsCustomerOrders)
                .HasForeignKey(x => x.CustomerCompanyID)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<CombinationFitting>()
            //    .HasOptional(x => x.DimSize)
            //    .WithMany(x => x.DimSizes)
            //    .HasForeignKey(x => x.DimSizeDimID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<CombinationFitting>()
                .HasRequired(x => x.DimCount)
                .WithMany(x => x.DimCounts)
                .HasForeignKey(x => x.DimCountDimID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NodeDetailFitting>()
                .HasRequired(x => x.Node)
                .WithMany(x => x.Nodes)
                .HasForeignKey(x => x.NodeFittingID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NodeDetailFitting>()
                .HasRequired(x => x.Detail)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.DetailFittingID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Company>()
                .HasRequired(x => x.LegalCity)
                .WithMany(x => x.LegalCompanies)
                .HasForeignKey(x => x.LegalCityCityID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Company>()
                .HasRequired(x => x.ActualCity)
                .WithMany(x => x.ActualCompanies)
                .HasForeignKey(x => x.ActualCityCityID)
                .WillCascadeOnDelete(false);

            //Многие-ко-многим в рамках одной сущности  - НЕ СТИРАТЬ - ПРИГОДИТСЯ!!!!!!!!!!!
            //modelBuilder.Entity<Company>()
            //    .HasMany(c => c.Sellers)
            //    .WithMany(c => c.Customers)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("SellerID");
            //        m.MapRightKey("CustomerID");
            //        m.ToTable("SellerCustomerCompanies");
            //    });


            //modelBuilder.Entity<BoolParameter>() //Связь один-ко-многим таблицы "Groups" с таблицей "GroupGroups"
            //            .HasMany(c => c.BoolParameterBoolParameters)    //в таблице "GroupGroups" много ссылок на таблицу "Group"
            //            .WithRequired(g => g.Child)   //с обязательным ненулевым навигационным свойством "Child"
            //            .HasForeignKey(g => g.ChildBoolParameterID) //связь через внешний ключ "ChildGroupID"
            //            .WillCascadeOnDelete(false);    //без каскадного удаления

        }

    }
}
