using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace NumberSpeaker.Common
{
    public class ValidatorFactory : ValidatorFactoryBase 
    {
        public ValidatorFactory()
        {
            //register the Validators
            SimpleIoc.Default.Register<IValidator<Member>, MemberValidator>();
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return SimpleIoc.Default.GetInstance(validatorType) as IValidator;
        }
    }
}
