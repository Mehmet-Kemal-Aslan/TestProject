using NUnit.Framework;
using JobApplication;
using JobApplication.Models;
using Moq;
using JobApplication.Services;
using System.Collections.Generic;
using FluentAssertions;
using System;

namespace Test
{
    public class UnitTest1
    {
        private Applicant Applicant;
        private List<string> TechStackList;
        private int YearsOfExperiences;

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid("")).Returns(true);


            var evaluator = new ApplicationEvaluator(null);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 17,
                    IdentityNumber = ""
                }

            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new System.Collections.Generic.List<string>()
                {
                    ""
                }

            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new System.Collections.Generic.List<string>()
                {
                     "C#", "RabbitMQ", "Microservice", "Visual Studio"
                },
                YearsOfExperience = 16

            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }



        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                }

            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.TranferredToHR, appResult);
            appResult.Should().Be(ApplicationResult.TranferredToHR);
        }

        [Test]
        public void Application_WithInOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant(){Age = 19},
            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.TranferredToCTO, appResult);
            appResult.Should().Be(ApplicationResult.TranferredToCTO);
        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");
            mockValidator.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant() { Age = 51 },
            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithNullApplicant_ThrowsArgumentNullException()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application();


            //Action
            System.Action appResultAction = () => evaluator.Evaluate(form);
            //var appResult = evaluator.Evaluate(form);


            //Assert
            appResultAction.Should().Throw<ArgumentNullException>();
            //mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");



            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = ""
                }
            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()));

        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");



            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new Application()
            {
                Applicant = new Applicant()
                {
                    Age = 15
                }
            };


            //Action
            var appResult = evaluator.Evaluate(form);


            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), Times.Never());

        }
    }
}