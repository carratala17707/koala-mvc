using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Koala.Helpers
{
    public class Util
    {
        public static string ValidationErrors(
                    System.Data.Entity.Validation.DbEntityValidationException ex, string message = "")
        {
            string validationErrors = string.Empty;
            foreach (var eve in ex.EntityValidationErrors)
            {
                validationErrors += string.Format(
                    "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    validationErrors += string.Format("- Property: \"{0}\", Error: \"{1}\"\n",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }
            return string.Format("{0} {1}", validationErrors, message);
        }
    }
}