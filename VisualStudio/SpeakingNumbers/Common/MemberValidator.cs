using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NumberSpeaker;
using NumberSpeaker.ViewModel;

namespace NumberSpeaker.Common
{
    public class MemberValidator : AbstractValidator<MyViewModel>
    {
        public MemberValidator()
        {
            RuleFor(member => member.TextForLabel1).NotEmpty();
        }
    }
}
