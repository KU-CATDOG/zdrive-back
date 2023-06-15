using Microsoft.EntityFrameworkCore;

namespace ZDrive.Extensions;

public static class DbContextExtensions
{
    public static void RevertChanges(this DbContext context)  
    {  
        foreach (var entry in context.ChangeTracker.Entries())  
        {  
            switch (entry.State)  
            {  
                case EntityState.Modified:  
                    entry.State = EntityState.Unchanged;  
                    break;  
                case EntityState.Added:  
                    entry.State = EntityState.Detached;  
                    break;    
                case EntityState.Deleted:  
                    entry.Reload();  
                    break;  
                default: break;  
            }  
        }   
    }  
}