# EFOptimisticConcurrencyResolution
Small demo project with DbUpdateConcurrency Entity Framework exception reproducing and ways to resolve.

Articles used:
1. RowVersion vs ConcurrencyToken In EntityFramework/EFCore
   https://dotnetcoretutorials.com/2020/07/17/rowversion-vs-concurrencytoken-in-entityframework-efcore/
2. Handling Concurrency Conflicts (EF6)
   https://docs.microsoft.com/en-us/ef/ef6/saving/concurrency

In the DbContext recursion is used to recall SaveChanges()
