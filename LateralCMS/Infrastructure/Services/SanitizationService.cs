using LateralCMS.Application.DTOs;
using System.Text.Encodings.Web;

namespace LateralCMS.Infrastructure.Services;

public class SanitizationService
{
    public void Sanitize(List<CmsEventDto> events)
    {
        foreach (var evt in events)
        {
            evt.Type = evt.Type?.Trim().ToLowerInvariant();
            evt.Id = evt.Id?.Trim();

            if (evt.Payload != null)
            {
                SanitizeObjectStrings(evt.Payload);
            }
        }
    }

    public void SanitizeObjectStrings(object obj)
    {
        if (obj == null) return;

        var type = obj.GetType();
        if (type == typeof(string))
            return;

        foreach (var prop in type.GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite) continue;

            if (prop.PropertyType == typeof(string))
            {
                var value = prop.GetValue(obj) as string;
                if (value != null)
                {
                    var sanitized = HtmlEncoder.Default.Encode(value.Trim());
                    prop.SetValue(obj, sanitized);
                }
            }
            else if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum && prop.PropertyType != typeof(DateTime))
            {
                var nestedObj = prop.GetValue(obj);
                if (nestedObj != null)
                    SanitizeObjectStrings(nestedObj);
            }
        }
    }
}
