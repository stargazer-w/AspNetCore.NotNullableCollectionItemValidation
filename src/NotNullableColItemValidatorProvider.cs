using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Namotion.Reflection;

namespace Stargazer.Extensions.AspNetCore.NotNullableCollectionItemValidation
{
    public class NotNullableCollectionItemValidatorProvider : IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            if(context.ModelMetadata.ContainerType != null)
            {
                var itemType = context.ModelMetadata
                                      .ContainerType
                                      .ToContextualType()
                                      .GetProperty(context.ModelMetadata.Name!)
                                      !.AccessorType
                                      .EnumerableItemType;
                if(itemType is { IsValueType: false, Nullability: Nullability.NotNullable, })
                {
                    context.Results.Add(new ValidatorItem(context.ValidatorMetadata)
                    {
                        IsReusable = true,
                        Validator = new NotNullableCollectionItemValidator(),
                    });
                }
            }
        }
    }

    public class NotNullableCollectionItemValidator : IModelValidator
    {
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if(context.Model is not ICollection collectionModel)
                throw new ArgumentException("context.Model is not type of ICollection", nameof(context));

            int i = 0;
            foreach(var item in collectionModel)
            {
                if(item is null)
                    yield return new ModelValidationResult(i.ToString(), "item can not be null");
                i++;
            }
        }
    }
}
