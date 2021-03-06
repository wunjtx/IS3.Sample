﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using System.Net.Http;
using IS3.Sample.Model;
using System.Runtime.Remoting.Messaging;

namespace IS3.Sample.WebUser
{
    public class AuthorizationManager:ResourceAuthorizationManager
    {
        private BookDbContext db;

        public AuthorizationManager ()
        {
            var context = CallContext.GetData("BookDbContext") as BookDbContext;
            if (context == null)
            {
                context = new BookDbContext();
                CallContext.SetData("BookDbContext", context);
            }
            db = context;
        }
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            var permissions = db.Permission.Where(p => true).ToList();
            var sources =context.Resource;
            var claims = context.Principal;
            var aa = context.Action;
            if (permissions.Any(p => sources.Any(r => r.Value == p.ResourceName) && claims.HasClaim("role", p.RoleName)))
            {
                return Ok();
            }
            if (permissions.Any(p => context.Action.Any(a => a.Value == p.ActionName) && claims.HasClaim("role", p.RoleName)))
            {
                return Ok();
            }
            return Nok();

        }
    }
}
