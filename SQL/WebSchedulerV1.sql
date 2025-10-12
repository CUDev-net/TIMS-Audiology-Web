

IF OBJECT_ID('vw_web_scheduler_appointments','V') IS NOT NULL
	DROP VIEW [dbo].[vw_web_scheduler_appointments]
GO

create view [dbo].[vw_web_scheduler_appointments] as 
	select 'A-' + CONVERT(varchar(55),appt.ID) id,
			appt.ApptStart as start_date, 
			appt.ApptEnd as end_date,
			appt.PatID as patient_id,
			pat.FName + ' ' + pat.LName as patient_name,
			pat.SiteID as patient_site_id,
			prov.ID as provider_id,
			prov.FName + ' ' + prov.LName as provider_name,
			--prov.FName ProviderFirstName,
			--prov.LName ProviderLastName,
			prov.Color provider_color_value,
			apptstat.ID status_id,
			apptstat.Name status_name,
			apptstat.Show status_show,
			--apptstat.IconId StatusIconId,
			apptsite.ID site_id,
			apptsite.Name site_name,
			apptsite.Color site_color_value,
			--apptres.ID ResourceId,
			--apptres.Name ResourceName,
			--apptres.Color ResourceColorValue,
			appttype.ID appointment_type_id,
			appttype.Name appointment_type_name,
			appttype.Color appointment_type_color_value,
			--appttype.SLP AppointmentTypeIsSLP,
			appt.Notes notes
		from Appointment appt 
		left outer join Patient pat on appt.PatID = pat.ID
		left outer join Provider prov on appt.ProviderID = prov.ID
		left outer join AppointmentStatus apptstat on appt.ApptStatusID = apptstat.ID
		left outer join Site apptsite on appt.SiteID = apptsite.ID
		left outer join Resource apptres on appt.ResourceID = apptres.ID
		left outer join AppointmentType appttype on appt.ApptTypeID = appttype.ID
		where appt.RecurringIntervalID is NULL
GO

IF OBJECT_ID('vw_web_scheduler_recurring_schedules','V') IS NOT NULL
	DROP VIEW [dbo].[vw_web_scheduler_recurring_schedules]
GO

create view [dbo].[vw_web_scheduler_recurring_schedules] as
	select 'X-' + CONVERT(varchar(55),sch.ID) id,
			sch.ApptStart as start_date, 
			sch.ApptEnd as end_date,
			prov.ID as provider_id,
			prov.FName + ' ' + prov.LName as provider_name,
			prov.Color provider_color_value,
			apptsite.ID site_id,
			apptsite.Name site_name,
			apptsite.Color site_color_value,
			sch.Title as title,
			sch.Notes as notes,
			sch.Location as location,
			sch.Color as color,
			sch.RecurringIntervalID as recurring_interval_id,
			recurdata.IntervalType as interval_type,
			recurdata.SubType interval_sub_type,
			recurdata.T1Days day_interval,
			recurdata.T2WeekCnt week_interval,
			recurdata.T2Monday is_monday_set,
			recurdata.T2Tuesday is_tuesday_set,
			recurdata.T2Wednesday is_wednesday_set,
			recurdata.T2Thursday is_thursday_set,
			recurdata.T2Friday is_friday_set,
			recurdata.T2Saturday is_saturday_set,
			recurdata.T2Sunday is_sunday_set,
			recurdata.T34DayNum day_of_month,
			recurdata.T3MonthCnt month_interval,
			recurdata.T34DayTypeID day_of_week,
			recurdata.T34DayQualID day_qualifier,
			recurdata.T4MonthID [month],
			recurdata.EndType end_type,
			recurdata.EndOccurs end_after,
			recurdata.DtStart [recurrence_start_date],
			recurdata.DtEnd [recurrence_end_date]
	from Schedule sch
	left outer join Provider prov on sch.ProviderID = prov.ID
	left outer join Site apptsite on sch.SiteID = apptsite.ID
	join RecurringInterval recurdata on sch.RecurringIntervalID = recurdata.ID
	where sch.RecurringIntervalID != 0
GO

IF OBJECT_ID('vw_web_scheduler_schedules','V') IS NOT NULL
	DROP VIEW [dbo].[vw_web_scheduler_schedules]
GO

create view [dbo].[vw_web_scheduler_schedules] as
	select 'X-' + CONVERT(varchar(55),sch.ID) id,
			sch.ApptStart as start_date, 
			sch.ApptEnd as end_date,
			prov.ID as provider_id,
			prov.FName + ' ' + prov.LName as provider_name,
			prov.Color provider_color_value,
			apptsite.ID site_id,
			apptsite.Name site_name,
			apptsite.Color site_color_value,
			sch.Title as title,
			sch.Notes as notes,
			sch.Location as location,
			sch.Color as color
	from Schedule sch
	left outer join Provider prov on sch.ProviderID = prov.ID
	left outer join Site apptsite on sch.SiteID = apptsite.ID
	where sch.RecurringIntervalID = 0
GO