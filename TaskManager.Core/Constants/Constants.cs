namespace TaskManager.Core.Constants;

public static class Constants
{
    public const string DEFAULT_ORDER_VALUE = "desc";
    public const string DEFAULT_TASK_SORT_VALUE = "CreatedAt";
    public const string PASSWORD_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,100}$";

    public const int EMAIL_MAX_LENGTH = 255;

    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 100;

    public const int PASSWORD_MIN_LENGTH = 8;
    public const int PASSWORD_MAX_LENGTH = 100;

    public const int ROLE_MAX_LENGTH = 100;
    public const int AREA_MAX_LENGTH = 100;
    public const int ABOUT_MAX_LENGTH = 250;

    public const int TASK_TITLE_MIN_LENGTH = 3;
    public const int TASK_TITLE_MAX_LENGTH = 50;
    public const int TASK_DESCRIPTION_MAX_LENGTH = 500;
    public const int TASK_SEARCH_MAX_LENGTH = 100;

    public const int MIN_PAGE_SIZE = 1;
    public const int MAX_PAGE_SIZE = 100;
    public const int MAX_TASK_DELETE_BATCH_SIZE = 100;

    public const int JWT_EXPIRATION_HOURS = 2;
    public const int JWT_MIN_KEY_BYTES = 32;
}
