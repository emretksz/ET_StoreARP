using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;
using System;
using System.Linq;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect: MethodInterception
    {

        //validatorler type olarak gelir.
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {

            //type kontrol
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Doğrulama sınıfı değil");
            }

            _validatorType = validatorType;
        }
      
        protected override void OnBefore(IInvocation invocation)/// invocation) metot
        {

            //reflection
            var validator = (IValidator)Activator.CreateInstance(_validatorType);

            //_validatorType.BaseType.GetGenericArguments= productvalidationun basetype'ın generic yapısını getirir
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];

            //parametrelerini buluyorum
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}
