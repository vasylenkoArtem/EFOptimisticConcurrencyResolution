using System;
using System.Linq;
using EfConcurrencyHandling.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfConcurrencyHandling.Data
{
    public class StoreContext : DbContext
    {
        public static string ConnectionString = "Server=localhost;Database=Store;Trusted_Connection=True;";

        public StoreContext() : base()
        {
        }

        public DbSet<Product> Products { get; set; }


        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the database.
        ///     Return 0 if failed
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);

                return ResolveDbConcurrencyExceptionClientWin(ex);
                
                // return ResolveDbConcurrencyExceptionDatabaseWin(ex);
                
                // return ResolveDbConcurrencyExceptionCustomResult(ex);
                
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex);
            }

            return 0;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        private int ResolveDbConcurrencyExceptionDatabaseWin(DbUpdateConcurrencyException ex)
        {
            // Update the values of the entity that failed to save from the database
            // to save FROM DATABASE
            ex.Entries.Single().Reload();

            return SaveChanges();
        }

        private int ResolveDbConcurrencyExceptionClientWin(DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();

            // reload original values of the proxy object
            // to update one more time without errors
            // with NEW values
            entry.OriginalValues.SetValues(entry.GetDatabaseValues());

            return SaveChanges();
        }
        
        private int ResolveDbConcurrencyExceptionCustomResult(DbUpdateConcurrencyException ex)
        {
            // Get the current entity values and the values in the database
            var entry = ex.Entries.Single();
            var currentValues = entry.CurrentValues;
            var databaseValues = entry.GetDatabaseValues();

            // Choose an initial set of resolved values. In this case we
            // make the default be the values currently in the database.
            var resolvedValues = databaseValues.Clone();

            // Have the user choose what the resolved values should be
            HaveUserResolveConcurrency(currentValues, databaseValues, resolvedValues);

            // Update the original values with the database values and
            // the current values with whatever the user choose.
            entry.OriginalValues.SetValues(databaseValues);
            entry.CurrentValues.SetValues(resolvedValues);

            return SaveChanges();
        }
        
        public void HaveUserResolveConcurrency(PropertyValues currentValues,
            PropertyValues databaseValues,
            PropertyValues resolvedValues)
        {
            // Show the current, database, and resolved values to the user and have
            // them edit the resolved values to get the correct resolution.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Check for concurrency 
            modelBuilder.Entity<Product>()
                .Property(p => p.RowVersion)
                .IsRowVersion();
            
            base.OnModelCreating(modelBuilder);
        }
    }
    
}