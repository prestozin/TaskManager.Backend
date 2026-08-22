namespace TaskManager.Core.Constants;

public static class Messages
{
    public const string TASK_TITLE_REQUIRED = "O título da tarefa é obrigatório.";
    public const string TASK_TITLE_LENGTH = "O título deve ter entre 3 50 caracteres.";

    public const string TASK_DESCRIPTION_MAX_LENGTH = "A descrição não pode exceder 500 caracteres.";

    public const string TASK_ID_REQUIRED = "O Id da tarefa é obrigatório.";
    public const string TASK_STATUS_INVALID = "O status informado é inválido.";
    public const string TASK_PRIORITY_INVALID = "A prioridade informada é inválido.";

    public const string TASK_NOT_FOUND = "Nenhuma tarefa encontrada";
    public const string TASK_FETCH_FAILED = "Falha ao buscar tarefa";

    public const string TASK_CREATED_SUCCESSFULLY = "Tarefa criada com sucesso";
    public const string TASK_CREATION_FAILED = "Falha ao criar tarefa";

    public const string TASK_UPDATED_SUCCESSFULLY = "Tarefa atualizada com sucesso";
    public const string TASK_UPDATE_FAILED = "Falha ao atualizar tarefa";

    public const string TASK_DELETED_SUCCESSFULLY = "Tarefa deletada com sucesso";
    public const string TASK_DELETION_FAILED = "Falha ao deletar tarefa";

    public const string TASK_ALREADY_EXISTS = "Tarefa já existe";

    public const string USER_NOT_FOUND = "Usuário não encontrado";
    public const string USER_ALREADY_EXISTS = "Usuário já cadastrado";

    public const string USER_CREATED_SUCCESSFULLY = "Usuário criado com sucesso";
    public const string USER_CREATION_FAILED = "Falha ao criar usuário";

    public const string USER_OR_PASSWORD_INVALID = "Usuário ou senha inválido.";

    public const string EMAIL_REQUIRED = "O e-mail é obrigatório.";
    public const string PASSWORD_REQUIRED = "A senha é obrigatória.";

    public const string EMAIL_INVALID = "Email inválido";
    public const string EMAIL_MAX_LENGTH = "O e-mail não pode exceder {0} caracteres.";

    public const string NAME_REQUIRED = "O nome é obrigatório.";
    public const string NAME_LENGTH = "O nome deve ter entre 3 e 100 caracteres.";

    public const string PASSWORD_RULES = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial.";

    public const string LOGIN_SUCCESSFULLY = "Login realizado com sucesso!";
}
