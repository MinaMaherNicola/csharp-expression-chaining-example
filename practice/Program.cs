using practice;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Practice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var books = Search(fromDate: new DateOnly(2005, 01, 01), toDate: new DateOnly(2010, 12, 31));
            books.ForEach(book => Console.WriteLine(book.Name));
        }

        public static List<Book> Search(string? name = null,
                                        decimal? fromPrice = null,
                                        decimal? toPrice = null,
                                        DateOnly? fromDate = null,
                                        DateOnly? toDate = null,
                                        bool? isDiscount = null,
                                        decimal? discountAmount = null)
        {
            List<Book> books = new()
            {
                new Book() { Name = "A", Price = 20, ReleaseDate = new DateOnly(2001, 01, 01), Discount = false },
                new Book() { Name = "B", Price = 50, ReleaseDate = new DateOnly(2001, 05, 01), Discount = true, Percentage = 0.1M },
                new Book() { Name = "C", Price = 100, ReleaseDate = new DateOnly(2001, 06, 01), Discount = false },
                new Book() { Name = "D", Price = 150, ReleaseDate = new DateOnly(2005, 01, 01), Discount = false },
                new Book() { Name = "E", Price = 180, ReleaseDate = new DateOnly(2010, 01, 01), Discount = false },
                new Book() { Name = "F", Price = 182.5M, ReleaseDate = new DateOnly(2010, 12, 31), Discount = true, Percentage = 0.5M },
                new Book() { Name = "G", Price = 199.99M, ReleaseDate = new DateOnly(2020, 01, 01), Discount = false },
                new Book() { Name = "H", Price = 299.99M, ReleaseDate = new DateOnly(2021, 01, 01), Discount = true, Percentage = 0.4M },
            };

            // This does the same as all the code below
            //var result = books.FindAll(x => (name == null || x.Name == name)
            //                             && (fromPrice == null || x.Price >= fromPrice)
            //                             && (toPrice == null || x.Price <= toPrice)
            //                             && (fromDate == null || x.ReleaseDate >= fromDate)
            //                             && (toDate == null || x.ReleaseDate <= toDate)
            //                             && (isDiscount == null || x.Discount == isDiscount)
            //                             && (discountAmount == null || x.Percentage == discountAmount)
            //                             );
            var expressionResult = GetExpression(name, fromPrice, toPrice, fromDate, toDate, isDiscount, discountAmount, books);
            return books.AsQueryable().Where(expressionResult).ToList();
        }

        private static Expression<Func<Book, bool>> GetExpression(string? name,
                                               decimal? fromPrice,
                                               decimal? toPrice,
                                               DateOnly? fromDate,
                                               DateOnly? toDate,
                                               bool? isDiscount,
                                               decimal? discountAmount,
                                               List<Book> books)
        {
            var bookParameter = Expression.Parameter(typeof(Book));
            Expression result = bookParameter;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (result.NodeType == ExpressionType.Parameter)
                {
                    result = SearchWithNameExpression(name, bookParameter);
                }
            }
            if (fromDate.HasValue)
            {
                if (result.NodeType == ExpressionType.Parameter)
                {
                    result = SearchWithFromDateExpression(fromDate.Value, bookParameter);
                }
                else
                {
                    result = Expression.AndAlso(result, SearchWithFromDateExpression(fromDate.Value, bookParameter));
                }
            }
            if (toDate.HasValue)
            {
                if (result.NodeType == ExpressionType.Parameter)
                {
                    result = SearchWithToDateExpression(toDate.Value, bookParameter);
                }
                else
                {
                    result = Expression.AndAlso(result, SearchWithToDateExpression(toDate.Value, bookParameter));
                }
            }
            return Expression.Lambda<Func<Book, bool>>(result, bookParameter);
        }

        private static Expression SearchWithNameExpression(string name, ParameterExpression parameter)
        {
            var nameProperty = Expression.Property(parameter, "Name");
            return Expression.Equal(nameProperty, Expression.Constant(name));
        }

        private static Expression SearchWithFromDateExpression(DateOnly fromDate, ParameterExpression parameter)
        {
            var releaseDateProperty = Expression.Property(parameter, "ReleaseDate");
            return Expression.GreaterThanOrEqual(releaseDateProperty, Expression.Constant(fromDate));
        }
        private static Expression SearchWithToDateExpression(DateOnly toDate, ParameterExpression parameter)
        {
            var releaseDateProperty = Expression.Property(parameter, "ReleaseDate");
            return Expression.LessThanOrEqual(releaseDateProperty, Expression.Constant(toDate));
        }
    }
}