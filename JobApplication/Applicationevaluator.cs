using JobApplication.Models;
using JobApplication.Services;

namespace JobApplication
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearsOfExperiences = 15;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };
        private IIdentityValidator identityValidator;

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            //this.@object = @object;
            this.identityValidator = identityValidator; 
        }

        public ApplicationResult Evaluate(Application form)
        {
            if (form.Applicant == null)
                throw new ArgumentNullException();

            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

            var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);
            if (identityValidator.CountryDataProvider.CountryData.Country != "TURKEY")
            {
                return ApplicationResult.TranferredToCTO;
            }

            if (!validIdentity)
                return ApplicationResult.TranferredToHR;

            var sr = GetTechStackSimilarityRate(form.TechStackList);
            if(sr < 25)
                return ApplicationResult.AutoRejected;
            if (sr > 75 && form.YearsOfExperience >=  autoAcceptedYearsOfExperiences)
                return ApplicationResult.AutoAccepted;


            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount =
                techStacks
                    .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                    .Count();
            return (int)((double)matchedCount / techStackList.Count)*100;
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        TranferredToHR,
        TranferredToLead,
        TranferredToCTO,
        AutoAccepted
    }
}