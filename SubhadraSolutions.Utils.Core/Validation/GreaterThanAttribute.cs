namespace SubhadraSolutions.Utils.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Compare two date properties and check one is greater than other.
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    public class GreaterThanAttribute : ValidationAttribute
    {
        private readonly string comparisonProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="comparisonProperty">The comparison property.</param>
        public GreaterThanAttribute(string comparisonProperty)
        {
            this.comparisonProperty = comparisonProperty;
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="ValidationResult"></see> class.
        /// </returns>
        /// <exception cref="ArgumentException">Property with this name not found</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ErrorMessage = this.ErrorMessageString;
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(this.comparisonProperty);

            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue < comparisonValue)
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}