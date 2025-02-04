using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class Communication
{
    public Guid CommunicationId { get; set; }

    public Guid ClientId { get; set; }

    public Guid PractitionerId { get; set; }

    public Guid AppointmentId { get; set; }

    public string Subject { get; set; }

    public string Type { get; set; }

    public string Sender { get; set; }

    public string Recipient { get; set; }

    public DateTime SentAt { get; set; }

    public string CommunicationMethod { get; set; }

    public virtual Appointment Appointment { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual CompanyClient Practitioner { get; set; }
}
