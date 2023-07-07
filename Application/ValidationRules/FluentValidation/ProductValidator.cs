using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
  public class ProductValidator:AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ModelName).NotEmpty();
            RuleFor(p => p.ModelCode).NotEmpty();
            //RuleFor(p => p. Age).NotEmpty();
            //RuleFor(p => p.ModelColor).NotEmpty();
            //RuleFor(p => p.Title).NotEmpty();
            //RuleFor(p => p.SubTitle).NotEmpty();
            //RuleFor(p => p.Price).NotEmpty();
            //RuleFor(p => p.ProductAddresses.Adress).NotEmpty();
            //RuleFor(p => p.ProductAddresses.Province.Name).NotEmpty();
            //RuleFor(p => p.ProductAddresses.Town.Name).NotEmpty();
            //RuleFor(p => p.ProductAddresses.District.Name).MinimumLength(2);
            //RuleFor(p => p.ProductAddresses.Province.Name).MinimumLength(2);
            //RuleFor(p => p.ProductAddresses.Town.Name).MinimumLength(2);

        }

    }
}
