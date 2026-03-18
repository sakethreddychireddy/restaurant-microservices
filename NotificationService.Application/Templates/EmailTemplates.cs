namespace NotificationService.Application.Templates
{
    public static class EmailTemplates
    {
        private static string BaseTemplate(string content) => $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <style>
    * {{ box-sizing: border-box; margin: 0; padding: 0; }}
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #faf8f5; margin: 0; padding: 20px 0; }}
    .container {{ max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 20px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08); }}

    /* Header */
    .header {{ background: linear-gradient(135deg, #c17f3d 0%, #8b5e2a 100%); padding: 40px 32px; text-align: center; }}
    .header .logo {{ font-size: 36px; margin-bottom: 8px; }}
    .header h1 {{ color: #ffffff; font-size: 26px; font-weight: 700; letter-spacing: -0.5px; margin-bottom: 4px; }}
    .header p {{ color: #f5e6d3; font-size: 13px; letter-spacing: 2px; text-transform: uppercase; }}

    /* Body */
    .body {{ padding: 36px 32px; }}
    .greeting {{ font-size: 22px; color: #2c2c2c; font-weight: 700; margin-bottom: 8px; }}
    .message {{ font-size: 15px; color: #6b6b6b; line-height: 1.7; margin-bottom: 28px; }}

    /* Info box */
    .info-box {{ background: #faf8f5; border-radius: 14px; padding: 22px 24px; margin-bottom: 24px; border: 1px solid #ede8e0; }}
    .info-box-title {{ font-size: 11px; color: #9b8b7a; text-transform: uppercase; letter-spacing: 1.5px; font-weight: 700; margin-bottom: 16px; }}
    .info-row {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }}
    .info-row:last-child {{ margin-bottom: 0; }}
    .info-label {{ font-size: 13px; color: #9b8b7a; }}
    .info-value {{ font-size: 13px; color: #2c2c2c; font-weight: 600; text-align: right; max-width: 60%; }}

    /* Items table */
    .items-table {{ width: 100%; border-collapse: collapse; margin-top: 4px; }}
    .items-table th {{ text-align: left; font-size: 11px; color: #9b8b7a; text-transform: uppercase; letter-spacing: 1px; padding: 8px 0; border-bottom: 2px solid #ede8e0; }}
    .items-table td {{ padding: 12px 0; font-size: 14px; color: #2c2c2c; border-bottom: 1px solid #f5f0ea; }}
    .item-name {{ font-weight: 600; }}
    .item-qty {{ text-align: center; color: #9b8b7a; }}
    .item-price {{ text-align: right; color: #6b6b6b; }}
    .item-subtotal {{ text-align: right; font-weight: 600; }}
    .total-row td {{ font-weight: 700; color: #c17f3d; font-size: 16px; border-bottom: none; padding-top: 16px; border-top: 2px solid #ede8e0; }}

    /* Status badge */
    .status-badge {{ display: inline-block; padding: 6px 14px; border-radius: 20px; font-size: 13px; font-weight: 700; }}

    /* Progress tracker */
    .progress-section {{ margin-bottom: 28px; }}
    .progress-title {{ font-size: 11px; color: #9b8b7a; text-transform: uppercase; letter-spacing: 1.5px; font-weight: 700; margin-bottom: 16px; }}
    .progress-steps {{ display: flex; justify-content: space-between; position: relative; }}
    .progress-steps::before {{ content: ''; position: absolute; top: 16px; left: 0; right: 0; height: 2px; background: #ede8e0; z-index: 0; }}
    .step {{ text-align: center; flex: 1; position: relative; z-index: 1; }}
    .step-circle {{ width: 32px; height: 32px; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 8px; font-size: 14px; }}
    .step-circle.active {{ background: #c17f3d; }}
    .step-circle.done {{ background: #22c55e; }}
    .step-circle.inactive {{ background: #ede8e0; }}
    .step-label {{ font-size: 11px; color: #9b8b7a; font-weight: 600; }}
    .step-label.active {{ color: #c17f3d; }}
    .step-label.done {{ color: #22c55e; }}

    /* Highlight box */
    .highlight-box {{ border-radius: 14px; padding: 20px 24px; margin-bottom: 24px; text-align: center; }}
    .highlight-box .big-emoji {{ font-size: 48px; margin-bottom: 12px; }}
    .highlight-box .highlight-text {{ font-size: 18px; font-weight: 700; color: #2c2c2c; margin-bottom: 4px; }}
    .highlight-box .highlight-sub {{ font-size: 13px; color: #9b8b7a; }}

    /* Divider */
    .divider {{ border: none; border-top: 1px solid #ede8e0; margin: 24px 0; }}

    /* Tips */
    .tip-box {{ background: #fff8f0; border-radius: 12px; padding: 16px 20px; margin-bottom: 24px; border-left: 4px solid #c17f3d; }}
    .tip-box p {{ font-size: 13px; color: #6b6b6b; line-height: 1.6; margin: 0; }}
    .tip-box strong {{ color: #c17f3d; }}

    /* Footer */
    .footer {{ background: #faf8f5; padding: 28px 32px; text-align: center; border-top: 1px solid #ede8e0; }}
    .footer .social {{ margin-bottom: 12px; font-size: 20px; letter-spacing: 8px; }}
    .footer p {{ font-size: 12px; color: #b0a090; line-height: 1.8; }}
    .footer .brand {{ font-size: 13px; color: #c17f3d; font-weight: 700; margin-bottom: 8px; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <div class='logo'>🍽️</div>
      <h1>Mithila</h1>
      <p>Crafted with Passion</p>
    </div>
    <div class='body'>
      {content}
    </div>
    <div class='footer'>
      <div class='social'>🍕 🍝 🥗</div>
      <p class='brand'>Mithila Restaurant</p>
      <p>Thank you for choosing us.<br/>
      This is an automated message — please do not reply directly.<br/>
      For support, visit us or call our helpline.</p>
    </div>
  </div>
</body>
</html>";

        // ── Order Placed ───────────────────────────────────────────
        public static string OrderPlaced(
            string userName,
            Guid orderId,
            decimal totalPrice,
            string deliveryAddress,
            List<(string Name, decimal UnitPrice, int Quantity)> items)
        {
            var itemRows = string.Join("", items.Select(i => $@"
            <tr>
                <td class='item-name'>{i.Name}</td>
                <td class='item-qty'>{i.Quantity}x</td>
                <td class='item-price'>${i.UnitPrice:F2}</td>
                <td class='item-subtotal'>${i.UnitPrice * i.Quantity:F2}</td>
            </tr>"));

            var shortId = orderId.ToString().ToUpper()[..8];

            var content = $@"
            <p class='greeting'>Hey {userName}! 🎉</p>
            <p class='message'>
                Your order has been placed successfully!
                We've received it and our kitchen will start preparing it shortly.
            </p>

            <div class='highlight-box' style='background:#fff8f0;'>
                <div class='big-emoji'>🎊</div>
                <div class='highlight-text'>Order Confirmed!</div>
                <div class='highlight-sub'>Estimated delivery: 30–45 minutes</div>
            </div>

            <div class='info-box'>
                <div class='info-box-title'>Order Summary</div>
                <div class='info-row'>
                    <span class='info-label'>Order ID</span>
                    <span class='info-value' style='font-family:monospace'>#{shortId}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Placed At</span>
                    <span class='info-value'>{DateTime.UtcNow:MMM dd, yyyy hh:mm tt} UTC</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Delivery Address</span>
                    <span class='info-value'>{deliveryAddress}</span>
                </div>
            </div>

            <div class='info-box'>
                <div class='info-box-title'>Items Ordered</div>
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
                            <td colspan='3'>Order Total</td>
                            <td style='text-align:right'>${totalPrice:F2}</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class='progress-section'>
                <div class='progress-title'>Order Progress</div>
                <div class='progress-steps'>
                    <div class='step'>
                        <div class='step-circle done'>✓</div>
                        <div class='step-label done'>Placed</div>
                    </div>
                    <div class='step'>
                        <div class='step-circle active'>⏳</div>
                        <div class='step-label active'>Confirmed</div>
                    </div>
                    <div class='step'>
                        <div class='step-circle inactive'>👨‍🍳</div>
                        <div class='step-label'>Preparing</div>
                    </div>
                    <div class='step'>
                        <div class='step-circle inactive'>🚀</div>
                        <div class='step-label'>Delivered</div>
                    </div>
                </div>
            </div>

            <div class='tip-box'>
                <p><strong>📱 Track your order</strong> — Log in to your account to track
                your order in real time. We'll also email you when the status changes!</p>
            </div>";

            return BaseTemplate(content);
        }

        // ── Order Status Changed ───────────────────────────────────
        public static string OrderStatusChanged(
            string userName,
            Guid orderId,
            string newStatus)
        {
            var (emoji, headline, message, color, bgColor, steps) = newStatus switch
            {
                "Confirmed" => (
                    "✅", "Order Confirmed!",
                    "Great news! Your order has been confirmed and our kitchen is gearing up to prepare your meal.",
                    "#22c55e", "#f0fdf4",
                    (done: new[] { "Placed", "Confirmed" }, active: "Preparing", inactive: new[] { "Delivered" })
                ),
                "Preparing" => (
                    "👨‍🍳", "Being Prepared!",
                    "Our chefs are working their magic right now. Your delicious meal is being freshly prepared just for you!",
                    "#f59e0b", "#fffbeb",
                    (done: new[] { "Placed", "Confirmed", "Preparing" }, active: "Delivering", inactive: new[] { "Delivered" })
                ),
                "Delivered" => (
                    "🎉", "Order Delivered!",
                    "Your order has arrived! We hope you enjoy every bite. Thank you for dining with Mithila!",
                    "#22c55e", "#f0fdf4",
                    (done: new[] { "Placed", "Confirmed", "Preparing", "Delivered" }, active: "", inactive: Array.Empty<string>())
                ),
                "Cancelled" => (
                    "❌", "Order Cancelled",
                    "We're sorry to inform you that your order has been cancelled. If you didn't request this, please contact us immediately.",
                    "#ef4444", "#fef2f2",
                    (done: Array.Empty<string>(), active: "Cancelled", inactive: new[] { "Preparing", "Delivered" })
                ),
                _ => (
                    "📋", $"Status: {newStatus}",
                    $"Your order status has been updated to {newStatus}.",
                    "#6b7280", "#f9fafb",
                    (done: Array.Empty<string>(), active: newStatus, inactive: Array.Empty<string>())
                )
            };

            var shortId = orderId.ToString().ToUpper()[..8];

            // build progress steps
            var allSteps = new[] {
            ("Placed", "✓"),
            ("Confirmed", "✓"),
            ("Preparing", "👨‍🍳"),
            ("Delivered", "🎉")
        };

            var progressHtml = string.Join("", allSteps.Select(s =>
            {
                string circleClass, labelClass, icon;
                if (steps.done.Contains(s.Item1))
                {
                    circleClass = "done";
                    labelClass = "done";
                    icon = "✓";
                }
                else if (s.Item1 == steps.active)
                {
                    circleClass = "active";
                    labelClass = "active";
                    icon = s.Item2;
                }
                else
                {
                    circleClass = "inactive";
                    labelClass = "";
                    icon = s.Item2;
                }
                return $@"
                <div class='step'>
                    <div class='step-circle {circleClass}'>{icon}</div>
                    <div class='step-label {labelClass}'>{s.Item1}</div>
                </div>";
            }));

            var content = $@"
            <p class='greeting'>Hey {userName}!</p>
            <p class='message'>{message}</p>

            <div class='highlight-box' style='background:{bgColor};'>
                <div class='big-emoji'>{emoji}</div>
                <div class='highlight-text'>{headline}</div>
                <div class='highlight-sub'>Order #{shortId}</div>
            </div>

            <div class='info-box'>
                <div class='info-box-title'>Order Details</div>
                <div class='info-row'>
                    <span class='info-label'>Order ID</span>
                    <span class='info-value' style='font-family:monospace'>#{shortId}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Updated At</span>
                    <span class='info-value'>{DateTime.UtcNow:MMM dd, yyyy hh:mm tt} UTC</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Current Status</span>
                    <span class='info-value'>
                        <span class='status-badge'
                            style='background:{bgColor}; color:{color}; border:1px solid {color}40'>
                            {emoji} {newStatus}
                        </span>
                    </span>
                </div>
            </div>

            <div class='progress-section'>
                <div class='progress-title'>Order Progress</div>
                <div class='progress-steps'>
                    {progressHtml}
                </div>
            </div>

            {(newStatus == "Delivered" ? $@"
            <div class='tip-box'>
                <p><strong>😊 Enjoyed your meal?</strong> We'd love to hear from you!
                Your feedback helps us serve you better next time.</p>
            </div>" : newStatus == "Preparing" ? $@"
            <div class='tip-box'>
                <p><strong>⏱️ Almost there!</strong> Your order is being freshly prepared.
                Estimated delivery in 15–20 minutes.</p>
            </div>" : "")}";

            return BaseTemplate(content);
        }

        // ── Order Cancelled ────────────────────────────────────────
        public static string OrderCancelled(
            string userName,
            Guid orderId)
        {
            var shortId = orderId.ToString().ToUpper()[..8];

            var content = $@"
            <p class='greeting'>Hey {userName},</p>
            <p class='message'>
                We're sorry to let you know that your order has been cancelled.
                We hope to serve you again soon!
            </p>

            <div class='highlight-box' style='background:#fef2f2;'>
                <div class='big-emoji'>😔</div>
                <div class='highlight-text'>Order Cancelled</div>
                <div class='highlight-sub'>Order #{shortId}</div>
            </div>

            <div class='info-box'>
                <div class='info-box-title'>Cancelled Order</div>
                <div class='info-row'>
                    <span class='info-label'>Order ID</span>
                    <span class='info-value' style='font-family:monospace'>#{shortId}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Cancelled At</span>
                    <span class='info-value'>{DateTime.UtcNow:MMM dd, yyyy hh:mm tt} UTC</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Status</span>
                    <span class='info-value'>
                        <span class='status-badge' style='background:#fee2e2; color:#ef4444; border:1px solid #fca5a5'>
                            ❌ Cancelled
                        </span>
                    </span>
                </div>
            </div>

            <div class='tip-box'>
                <p><strong>🤔 Didn't cancel this order?</strong> Please contact us immediately
                so we can look into this for you. We apologize for any inconvenience.</p>
            </div>

            <p class='message' style='text-align:center; margin-top:8px;'>
                We hope to see you again soon! Browse our menu and
                place a new order whenever you're ready. 🍽️
            </p>";

            return BaseTemplate(content);
        }
    }
}