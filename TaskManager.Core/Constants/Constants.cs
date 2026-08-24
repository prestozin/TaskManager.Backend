namespace TaskManager.Core.Constants;

public static class Constants
{
    public static readonly string[] SORT_ACCEPTED_VALUES = new[] { "CreatedAt", "Priority", "Status" };
    public static readonly string[] ORDER_ACCEPTED_VALUES = new[] { "asc", "desc" };
    public const string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string PASSWORD_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,100}$";

    public const int EMAIL_MAX_LENGTH = 255;

    public const int NAME_MAX_LENGTH = 100;
    public const int NAME_MIN_LENGTH = 3;

    public const int TASK_DESCRIPTION_MIN_LENGTH = 10;
    public const int TASK_DESCRIPTION_MAX_LENGTH = 500;

    public const int TASK_TITLE_MIN_LENGTH = 3;
    public const int TASK_TITLE_MAX_LENGTH = 50;

}
