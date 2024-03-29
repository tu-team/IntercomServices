﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using iLexStudio.IntercomServices.Models;
using System.Web.Security;
using iLexStudio.IntercomServices.Models.Users;
using iLexStudio.IntercomServices.Models.Tickets;

namespace iLexStudio.IntercomServices.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<LoginContexts>(null);

                try
                {
                    var databaseIsNew = false;
                    using (var context = new LoginContexts())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            context.Database.Create();
                            //((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                            databaseIsNew = true;
                            //

                        }
                    }

                    using (var context = new IntercomContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            context.Database.Create();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    if (databaseIsNew)
                    {
                        if (!WebSecurity.UserExists("Admin"))
                        {
                            WebSecurity.CreateUserAndAccount("Admin", "Admin");
                        }

                        if (!Roles.RoleExists("Admin"))
                        {
                            Roles.CreateRole("Admin");
                            Roles.AddUserToRole("Admin", "Admin");
                        }
                        if (!Roles.RoleExists("Engineer"))
                        {
                            Roles.CreateRole("Engineer");
                            Roles.AddUserToRole("Admin", "Engineer");
                        }
                        if (!Roles.RoleExists("Operator"))
                        {
                            Roles.CreateRole("Operator");
                            Roles.AddUserToRole("Admin", "Operator");
                        }
                        if (!Roles.RoleExists("User"))
                        {
                            Roles.CreateRole("User");
                            Roles.AddUserToRole("Admin", "User");
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
