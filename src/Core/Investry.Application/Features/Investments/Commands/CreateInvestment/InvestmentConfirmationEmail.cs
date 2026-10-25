using Investry.Application.Contracts.Identity;
using Investry.Application.Models.Email;
using Investry.Domain.Entities;
using Investry.Domain.Enums;

namespace Investry.Application.Features.Investments.Commands.CreateInvestment
{
    internal static class InvestmentConfirmationEmail
    {
        public static Email Create(UserDto user, Project project, decimal amountToDeduct)
        {
            string extraDetails = "";

            switch (project.FundingModel)
            {
                case FundingModel.Reward:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            🎁 You will receive your selected reward once the campaign is completed.
                                        </p>";
                    break;

                case FundingModel.Equity:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            📊 You now own a share in this project based on your investment.
                                        </p>";
                    break;

                case FundingModel.Mudarabah:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            🤝 Your investment will generate returns based on the project's performance.
                                        </p>";
                    break;
            }

            return new Email
            {
                To = user.Email,
                Subject = "💰 Investment Successful",
                Body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                        </head>
                        <body style='font-family: Arial, sans-serif; background-color:#f4f4f4; padding:20px;'>

                            <div style='max-width:600px; margin:auto; background:#ffffff; padding:25px; border-radius:12px;'>

                                <h2 style='color:#2c3e50;'>💰 Investment Confirmed, {user?.FirstName + " " + user?.LastName ?? "Investor"}!</h2>

                                <p style='font-size:16px; color:#333;'>
                                    Your investment has been successfully processed.
                                </p>

                                <p style='font-size:16px; color:#333;'>
                                    <strong>Project:</strong> ""{project.Title}""
                                </p>

                                <p style='font-size:16px; color:#333;'>
                                    <strong>Amount:</strong> {amountToDeduct} EGP
                                </p>

                                <p style='font-size:16px; color:#333;'>
                                    <strong>Date:</strong> {DateTime.UtcNow:yyyy-MM-dd}
                                </p>

                                {extraDetails}

                                <p style='font-size:16px; color:#333;'>
                                    The funds have been securely placed in escrow and will be handled according to the campaign outcome.
                                </p>

                                <!-- CTA Button -->
                                <div style='text-align:center; margin:30px 0;'>
                                    <a href='https://investry-project.vercel.app/investor/investments'
                                       style='background-color:#2c3e50; color:#ffffff; padding:12px 20px; text-decoration:none; border-radius:6px; font-size:16px;'>
                                       View Your Investment
                                    </a>
                                </div>

                                <hr style='margin:20px 0;'>

                                <p style='font-size:14px; color:#777;'>
                                    If you have any questions, feel free to contact our support team anytime.
                                </p>

                                <p style='font-size:14px; color:#2c3e50; font-weight:bold;'>
                                    — InvesTry Team
                                </p>

                            </div>

                        </body>
                        </html>
                        "
            };
        }
    }
}
