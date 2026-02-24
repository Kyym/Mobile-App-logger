using Microsoft.AspNetCore.Mvc;
using MobileAppLogger.Models;

namespace MobileAppLogger.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult PostLog([FromBody] LogEntry logEntry)
    {
        try
        {
            var enrichedMessage = $"[Mobile App] {logEntry.Message} | User: {logEntry.UserId ?? "anonymous"} | Screen: {logEntry.Screen ?? "unknown"} | Device: {logEntry.DeviceInfo ?? "unknown"} | Version: {logEntry.AppVersion ?? "unknown"}";

            switch (logEntry.Level?.ToLower())
            {
                case "error":
                    _logger.LogError(enrichedMessage);
                    break;
                case "warning":
                    _logger.LogWarning(enrichedMessage);
                    break;
                case "debug":
                    _logger.LogDebug(enrichedMessage);
                    break;
                default:
                    _logger.LogInformation(enrichedMessage);
                    break;
            }

            if (logEntry.AdditionalData != null && logEntry.AdditionalData.Any())
            {
                _logger.LogInformation("Additional data: {@AdditionalData}", logEntry.AdditionalData);
            }

            return Ok(new { status = "logged", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process log entry");
            return StatusCode(500, new { error = "Failed to process log" });
        }
    }

    [HttpPost("batch")]
    public IActionResult PostBatchLogs([FromBody] BatchLogRequest batchRequest)
    {
        try
        {
            _logger.LogInformation("Received batch of {Count} logs for session {SessionId}",
                batchRequest.Logs.Count, batchRequest.SessionId ?? "unknown");

            foreach (var logEntry in batchRequest.Logs)
            {
                var enrichedMessage = $"[Batch] {logEntry.Message} | Session: {batchRequest.SessionId ?? "unknown"}";

                switch (logEntry.Level?.ToLower())
                {
                    case "error":
                        _logger.LogError(enrichedMessage);
                        break;
                    default:
                        _logger.LogInformation(enrichedMessage);
                        break;
                }
            }

            return Ok(new
            {
                status = "batch logged",
                count = batchRequest.Logs.Count,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process batch logs");
            return StatusCode(500, new { error = "Failed to process batch logs" });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    // 🆕 NEW: JavaScript injection endpoint for WordPress
    [HttpGet("inject")]
    public IActionResult GetInjectScript()
    {
        var script = $@"
// 📝 MobileAppLogger - WordPress Injector
(function() {{
    const API_URL = 'http://{Request.Host}/api/Log';
    const SESSION_ID = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    
    console.log('🔌 MobileAppLogger connecting to:', API_URL);
    
    function sendLog(level, message, screen, data = {{}}) {{
        // Try to get WordPress user info
        let userId = 'anonymous';
        let userEmail = '';
        try {{
            // Check for WordPress logged-in indicators
            if (document.body.classList.contains('logged-in')) {{
                userId = 'wp-user';
            }}
            // Look for user data in WordPress globals
            if (window.wpUser?.ID) {{
                userId = window.wpUser.ID;
                userEmail = window.wpUser.email;
            }}
            // Check for user meta in the DOM
            const userMeta = document.querySelector('meta[name=""user-id""]');
            if (userMeta) {{
                userId = userMeta.content;
            }}
        }} catch(e) {{}}
        
        const logEntry = {{
            level: level,
            message: message,
            userId: userId,
            screen: screen || window.location.pathname,
            deviceInfo: navigator.userAgent,
            appVersion: 'WordPress-Injector',
            additionalData: {{
                ...data,
                sessionId: SESSION_ID,
                url: window.location.href,
                title: document.title,
                referrer: document.referrer,
                userEmail: userEmail,
                timestamp: new Date().toISOString(),
                screenWidth: window.screen.width,
                screenHeight: window.screen.height,
                language: navigator.language
            }}
        }};
        
        // Send to your logger (use image hack for maximum compatibility)
        if (navigator.sendBeacon) {{
            // Use sendBeacon for page unload events
            navigator.sendBeacon(API_URL, JSON.stringify(logEntry));
        }} else {{
            // Fallback to fetch with no-cors
            fetch(API_URL, {{
                method: 'POST',
                headers: {{ 'Content-Type': 'application/json' }},
                body: JSON.stringify(logEntry),
                mode: 'no-cors',
                keepalive: true
            }}).catch(e => console.log('Log error (silent):', e));
        }}
    }}
    
    // Log page view
    sendLog('Information', 'Page loaded: ' + document.title, window.location.pathname);
    
    // Track all clicks on interactive elements
    document.addEventListener('click', function(e) {{
        let target = e.target;
        let interactive = target.tagName === 'A' || target.tagName === 'BUTTON' || 
                         target.closest('a') || target.closest('button') ||
                         target.role === 'button' || target.onclick;
        
        if (interactive) {{
            let link = target.closest('a') || target;
            sendLog('Information', 'Click: ' + (target.innerText || target.value || 'unknown'), 
                    window.location.pathname, {{
                element: target.tagName,
                text: (target.innerText || target.value || '').substring(0, 100),
                href: link.href || null,
                id: target.id || null,
                class: target.className || null
            }});
        }}
    }});
    
    // Track form submissions (especially login)
    document.addEventListener('submit', function(e) {{
        let form = e.target;
        let isLogin = form.id === 'loginform' || 
                     window.location.pathname.includes('login') ||
                     form.querySelector('input[type=""password""]');
        
        sendLog(isLogin ? 'Information' : 'Information', 
                isLogin ? '🔐 Login attempt' : '📝 Form submitted', 
                window.location.pathname,
                {{ 
                    isLogin: isLogin, 
                    formId: form.id,
                    formAction: form.action,
                    formMethod: form.method
                }});
    }});
    
    // Track page visibility changes (user leaving/returning)
    document.addEventListener('visibilitychange', function() {{
        if (document.hidden) {{
            sendLog('Information', 'Tab hidden', window.location.pathname, {{
                timeOnPage: performance.now() / 1000 + 's'
            }});
        }} else {{
            sendLog('Information', 'Tab visible', window.location.pathname);
        }}
    }});
    
    // Track errors
    window.addEventListener('error', function(e) {{
        sendLog('Error', 'JavaScript error', window.location.pathname, {{
            message: e.message,
            filename: e.filename,
            lineno: e.lineno,
            colno: e.colno
        }});
    }});
    
    console.log('✅ MobileAppLogger active on:', window.location.hostname);
    console.log('📊 Sending logs to:', API_URL);
    
    // Send confirmation log
    sendLog('Information', 'Logger injected successfully', 'injection');
}})();
";

        return Content(script, "application/javascript");
    }
}