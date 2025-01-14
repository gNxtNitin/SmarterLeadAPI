namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class UserFeedbackContent
    {
        private static readonly string htmlContent = @"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Thank You!</title>
                <!-- Bootstrap CSS -->
                <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" rel=""stylesheet"">
                <!-- Custom CSS -->
                <style>
                    body {
                        background: radial-gradient(circle at center, #e0f7fa, #b2ebf2);
                        font-family: 'Arial', sans-serif;
                        color: #34495e;
                        margin: 0;
                        padding: 0;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        overflow: hidden;
                    }
                    .thank-you-container {
                        text-align: center;
                        background: white;
                        border-radius: 15px;
                        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                        padding: 50px;
                        width: 90%;
                        max-width: 500px;
                        max-height: 500px;
                        position: relative;
                        animation: fadeIn 1.5s ease-in-out;
                    }
                    .thank-you-icon {
                        position: absolute;
                        top: -15%;
                        left: 50%;
                        transform: translateX(-50%);
                        width: 100px;
                        height: 100px;
                        background: linear-gradient(135deg, #0490a3, #b7daf6);
                        border-radius: 50%;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
                        animation: zoomIn 1s ease-in-out;
                    }
                    .thank-you-icon img {
                        width: 70%;
                        animation: pop 1s infinite ease-in-out alternate;
                    }
                    .thank-you-message h1 {
                        font-size: 2rem;
                        color: #00796b;
                        margin-top: 30px;
                        animation: slideDown 1.2s ease-in-out;
                    }
                    .thank-you-message p {
                        font-size: 1rem;
                        color: #555;
                        margin: 20px 0;
                    }
                    .thank-you-button a {
                        text-decoration: none;
                        font-weight: bold;
                        font-size: 1rem;
                        color: white;
                        background: #00796b;
                        padding: 12px 25px;
                        border-radius: 30px;
                        transition: transform 0.3s ease, box-shadow 0.3s ease;
                        display: inline-block;
                    }
                    .thank-you-button a:hover {
                        transform: scale(1.1);
                        box-shadow: 0 4px 15px rgba(0, 121, 107, 0.3);
                    }
                    @keyframes zoomIn {
                        from {
                            transform: translateX(-50%) scale(0.5);
                            opacity: 0;
                        }
                        to {
                            transform: translateX(-50%) scale(1);
                            opacity: 1;
                        }
                    }
                    @keyframes fadeIn {
                        from {
                            opacity: 0;
                            transform: translateY(20px);
                        }
                        to {
                            opacity: 1;
                            transform: translateY(0);
                        }
                    }
                    @keyframes slideDown {
                        from {
                            transform: translateY(-20px);
                            opacity: 0;
                        }
                        to {
                            transform: translateY(0);
                            opacity: 1;
                        }
                    }
                    @keyframes pop {
                        from {
                            transform: scale(1);
                        }
                        to {
                            transform: scale(1.1);
                        }
                    }
                </style>
            </head>
            <body>
                <div class=""thank-you-container"">
                    <!-- Animated Icon -->
                    <div class=""thank-you-icon"">
                        <img src=""https://cdn-icons-png.flaticon.com/512/845/845646.png"" alt=""Confirmed"">
                    </div>
                    <!-- Thank You Message -->
                    <div class=""thank-you-message"">
                        <h1>Thank You!</h1>
                        <p>{{dynamicMessage}}</p>
                    </div>
                    <!-- Button -->
                    <div class=""thank-you-button"">
                        {{dynamicLinkTag}}
                    </div>
                </div>
                <!-- Bootstrap JS -->
                <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js""></script>
            </body>
            </html>";

        public static string GetFeedbackContent(string responseType)
        {
            string dynamicUrlTag = string.Empty, dynamicMessage = string.Empty;

            if (!string.IsNullOrEmpty(responseType))
            {
                switch (responseType)
                {
                    case "Email Response-Interested":
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>Let’s Get Started</a>";
                        dynamicMessage = "Great to hear you're interested! We'll be in touch soon with all the details you need.";
                        break;

                    case "Email Response-Not Interested":
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>Reach Out Anytime</a>";
                        dynamicMessage = "No worries! Feel free to reach out anytime if your needs change. We'll be happy to assist you whenever you're ready.";
                        break;

                    case "Email Response-Already Insured":
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>View Our Services</a>";
                        dynamicMessage = "Thanks for your response! It looks like you're already covered, which is great news. If you ever have questions about your insurance or need assistance, feel free to contact us. We're here to help.";
                        break;

                    case "Email Response-Need More Info":
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>Get Detailed Info</a>";
                        dynamicMessage = "Thank you for reaching out! We understand that you need more information before making a decision. We're happy to assist with any questions you have and provide the necessary details to help you make the best choice.\r\n\r\n";
                        break;

                    case "Error":
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>Contact Us</a>";
                        dynamicMessage = "We’re sorry, but we couldn’t save your response at the moment.Please try again, or feel free to reach out to us.";
                        break;

                    default:
                        dynamicUrlTag = @"<a href='https://www.smarterlead.io'>Contact Us</a>";
                        dynamicMessage = "We appreciate your interest! If you’re unsure or need help, feel free to contact us anytime.";
                        break;
                }
            }

            return htmlContent.Replace("{{dynamicMessage}}", dynamicMessage).Replace("{{dynamicLinkTag}}", dynamicUrlTag);
        }

    }
}
