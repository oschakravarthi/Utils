namespace SubhadraSolutions.Utils.Validation
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// VAlidation attribute to checl list count > 0.
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    public class RequiredListAttribute : RequiredAttribute
    {
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true if validation is successful; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            return value is IEnumerable list && list.GetEnumerator().MoveNext();
        }
    }
}