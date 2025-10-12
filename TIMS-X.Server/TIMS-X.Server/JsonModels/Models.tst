
${
    Template(Settings settings)
    {
        settings
             .IncludeProject("TIMS-X.Core")
             .IncludeProject("TIMS-X.BLL")
             .IncludeProject("TIMS-X.DAL");
    }
}
$Classes(:AuditableEntity)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(:Entity)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(*Dto)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(CalendarSummaryItem)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(ScheduleItemSummary)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(ScheduleRecurringItemSummary)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(AppointmentItemSummary)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(ValidationResult)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]
$Classes(RecurringInterval)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(RecurringIntervalRemoved)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(ScheduleOpeningsSearchModel)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(ScheduleOpeningModel)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]
        $BaseClass[$Properties[
        public $name: $Type;]]]
    }]
$Classes(ApptRecurringInterval)[
    export class $Name { $Properties[
        public $name: $Type;]
		$BaseClass[$Properties[
        public $name: $Type;]]
    }]