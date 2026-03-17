namespace NotificationService.Application.Templates
{
    public static class EmailTemplates
    {
        private static string BaseTemplate(string content) => $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #faf8f5; margin: 0; padding: 0; }}
    .container {{ max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.08); }}
    .header {{ background: linear-gradient(135deg, #c17f3d 0%, #8b5e2a 100%); padding: 40px 32px; text-align: center; }}
    .header h1 {{ color: #ffffff; margin: 0; font-size: 28px; font-weight: 700; letter-spacing: -0.5px; }}
    .header p {{ color: #f5e6d3; margin: 8px 0 0; font-size: 14px; }}
    .body {{ padding: 32px; }}
    .greeting {{ font-size: 18px; color: #2c2c2c; font-weight: 600; margin-bottom: 8px; }}
    .message {{ font-size: 15px; color: #6b6b6b; line-height: 1.6; margin-bottom: 24px; }}
    .order-box {{ background: #faf8f5; border-radius: 12px; padding: 20px 24px; margin-bottom: 24px; border: 1px solid #ede8e0; }}
    .order-box h3 {{ margin: 0 0 16px; font-size: 14px; color: #9b8b7a; text-transform: uppercase; letter-spacing: 1px; }}
    .order-row {{ display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 14px; }}
    .order-row .label {{ color: #9b8b7a; }}
    .order-row .value {{ color: #2c2c2c; font-weight: 600; }}
    .items-table {{ width: 100%; border-collapse: collapse; margin-top: 8px; }}
    .items-table th {{ text-align: left; font-size: 12px; color: #9b8b7a; text-transform: uppercase; padding: 8px 0; border-bottom: 1px solid #ede8e0; }}
    .items-table td {{ padding: 10px 0; font-size: 14px; color: #2c2c2c; border-bottom: 1px solid #f5f0ea; }}
    .total-row td {{ font-weight: 700; color: #c17f3d; font-size: 16px; border-bottom: none; padding-top: 14px; }}
    .status-badge {{ display: inline-block; padding: 6px 16px; border-radius: 20px; font-size: 13px; font-weight: 700; }}
    .footer {{ background: #faf8f5; padding: 24px 32px; text-align: center; border-top: 1px solid #ede8e0; }}
    .footer p {{ margin: 0; font-size: 12px; color: #b0a090; line-height: 1.6; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>🍽️ Mithila</h1>
      <p>Crafted with Passion</p>
    </div>
    <div class='body'>
      {content}
    </div>
    <div class='footer'>
      <p>Thank you for choosing Mithila Restaurant.<br/>
      This is an automated email, please do not reply.</p>
    </div>
  </div>
</body>
</html>";

        public static string OrderPlaced(
            string userName,
            Guid orderId,
            decimal totalPrice,
            string deliveryAddress,
            List<(string Name, decimal UnitPrice, int Quantity)> items)
        {
            var itemRows = string.Join("", items.Select(i => $@"
                <tr>
                    <td>{i.Name}</td>
                    <td style='text-align:center'>{i.Quantity}</td>
                    <td style='text-align:right'>${i.UnitPrice:F2}</td>
                    <td style='text-align:right'>${i.UnitPrice * i.Quantity:F2}</td>
                </tr>"));

            var content = $@"
                <p class='greeting'>Hey {userName}! 🎉</p>
                <p class='message'>Your order has been placed successfully and is being prepared with love.</p>

                <div class='order-box'>
                    <h3>Order Details</h3>
                    <div class='order-row'>
                        <span class='label'>Order ID</span>
                        <span class='value'>#{orderId.ToString().ToUpper()[..8]}</span>
                    </div>
                    <div class='order-row'>
                        <span class='label'>Delivery Address</span>
                        <span class='value'>{deliveryAddress}</span>
                    </div>

                    <table class='items-table'>
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th style='text-align:center'>Qty</th>
                                <th style='text-align:right'>Price</th>
                                <th style='text-align:right'>Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>
                            {itemRows}
                            <tr class='total-row'>
                                <td colspan='3'>Total</td>
                                <td style='text-align:right'>${totalPrice:F2}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <p class='message'>We'll notify you as your order progresses. Estimated delivery time is 30-45 minutes.</p>";

            return BaseTemplate(content);
        }

        public static string OrderStatusChanged(
            string userName,
            Guid orderId,
            string newStatus)
        {
            var (emoji, message, color) = newStatus switch
            {
                "Confirmed" => ("✅", "Your order has been confirmed and is being prepared!", "#22c55e"),
                "Preparing" => ("👨‍🍳", "Our chefs are preparing your delicious meal right now!", "#f59e0b"),
                "Delivered" => ("🎉", "Your order has been delivered. Enjoy your meal!", "#22c55e"),
                "Cancelled" => ("❌", "Your order has been cancelled.", "#ef4444"),
                _ => ("📋", $"Your order status has been updated to {newStatus}.", "#6b7280")
            };

            var content = $@"
                <p class='greeting'>Hey {userName}!</p>
                <p class='message'>{message}</p>

                <div class='order-box'>
                    <h3>Order Update</h3>
                    <div class='order-row'>
                        <span class='label'>Order ID</span>
                        <span class='value'>#{orderId.ToString().ToUpper()[..8]}</span>
                    </div>
                    <div class='order-row'>
                        <span class='label'>New Status</span>
                        <span class='value'>
                            <span class='status-badge' style='background:{color}20; color:{color}'>
                                {emoji} {newStatus}
                            </span>
                        </span>
                    </div>
                </div>";

            return BaseTemplate(content);
        }

        public static string OrderCancelled(
            string userName,
            Guid orderId)
        {
            var content = $@"
                <p class='greeting'>Hey {userName},</p>
                <p class='message'>We're sorry to let you know that your order has been cancelled.</p>

                <div class='order-box'>
                    <h3>Cancelled Order</h3>
                    <div class='order-row'>
                        <span class='label'>Order ID</span>
                        <span class='value'>#{orderId.ToString().ToUpper()[..8]}</span>
                    </div>
                    <div class='order-row'>
                        <span class='label'>Status</span>
                        <span class='value'>
                            <span class='status-badge' style='background:#fee2e2; color:#ef4444'>
                                ❌ Cancelled
                            </span>
                        </span>
                    </div>
                </div>

                <p class='message'>If you have any questions please contact us. We hope to serve you again soon!</p>";

            return BaseTemplate(content);
        }
    }
}