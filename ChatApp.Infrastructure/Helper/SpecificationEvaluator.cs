using ChatApp.Application.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Infrastructure.Helper
{ 
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery, ISpecification<T> Spec)
        {
            var Query = InputQuery;
            if (Spec.Criteria != null)Query = Query.Where(Spec.Criteria);
            if (Spec.Includes != null) Query = Spec.Includes.Aggregate(Query, (CurrentQuery, Include) => CurrentQuery.Include(Include));
            return Query;
        }
    }
}
