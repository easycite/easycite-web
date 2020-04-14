using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCiteLib.Models
{
    public class Results<T> where T : new()
    {
        public T Data { get; set; } = new T();

        public bool HasProblem => HasException || HasError;

        #region Exceptions
        public List<Exception> Exceptions { get; } = new List<Exception>();
        public bool HasException => Exceptions.Any();

        public void Merge<U>(Results<U> results) where U: new()
        {
            Exceptions.AddRange(results.Exceptions);
            ValidationErrors.AddRange(results.ValidationErrors);
        }

        public void AddException(Exception exception)
        {
            Exceptions.Add(exception);
        }
        #endregion

        #region Validation
        public List<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
        public bool HasError => ValidationErrors.Any();
        public void AddError(string message) => AddError(null, message);
        public void AddError(string property, string message) {
            ValidationErrors.Add(new ValidationError{
                Property = property,
                Message = message
            });
        }
        #endregion
    }

    public class ValidationError
    {
        public string Property { get; set; }
        public string Message { get; set; }
    }
}