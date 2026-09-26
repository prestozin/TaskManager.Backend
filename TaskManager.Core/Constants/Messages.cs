namespace TaskManager.Core.Constants;

public static class Messages
{
    public const string DEFAULT_SUCCESS = "Operação realizada com sucesso.";
    public const string DEFAULT_FAILURE = "Não foi possível concluir a operação.";
    public const string VALIDATION_ERROR = "Erro de validação.";
    public const string UNEXPECTED_ERROR = "Ocorreu um erro inesperado.";
    public const string UNAUTHORIZED = "Usuário não autenticado.";
    public const string TOO_MANY_REQUESTS = "Muitas tentativas. Aguarde um momento e tente novamente.";

    public const string INVALID_CREDENTIALS = "E-mail ou senha inválidos.";
    public const string PASSWORD_INVALID = "Senha incorreta.";
    public const string NEW_PASSWORD_MUST_BE_DIFFERENT = "A nova senha deve ser diferente da senha atual.";
    public const string PASSWORD_RULES = "A senha deve ter entre 8 e 100 caracteres e conter letra maiúscula, letra minúscula, número e caractere especial.";
    public const string LOGIN_SUCCESSFULLY = "Login realizado com sucesso.";

    public const string INVALID_DATE_RANGE = "A data inicial não pode ser posterior à data final.";
    public const string JWT_CONFIGURATION_INVALID = "A configuração do JWT é inválida.";

    public const string FIELD_REQUIRED = "O campo {0} é obrigatório.";
    public const string FIELD_INVALID = "O campo {0} é inválido.";

    public const string FIELD_LENGTH = "O campo {0} deve ter entre {1} e {2} caracteres.";
    public const string FIELD_MIN_LENGTH = "O campo {0} deve ter pelo menos {1} caracteres.";
    public const string FIELD_MAX_LENGTH = "O campo {0} deve ter no máximo {1} caracteres.";

    public const string FIELD_MINIMUM_VALUE = "O campo {0} deve ser maior ou igual a {1}.";
    public const string FIELD_RANGE = "O campo {0} deve ter um valor entre {1} e {2}.";

    public const string RESOURCE_NOT_FOUND = "Não foi possível localizar {0}.";
    public const string RESOURCE_ALREADY_EXISTS = "{0} já está cadastrado.";

    public const string OPERATION_SUCCESS = "{0} com sucesso.";
    public const string OPERATION_FAILED = "Não foi possível {0}.";

    public const string AT_LEAST_ONE_ITEM = "Informe pelo menos um item em {0}.";
    public const string MAXIMUM_ITEMS = "O campo {0} aceita no máximo {1} itens.";
    public const string DUPLICATE_ITEMS_NOT_ALLOWED = "O campo {0} não pode conter itens duplicados.";

    public const string TASKS_DELETED_SUCCESSFULLY = "{0} tarefa(s) excluída(s) com sucesso.";
    public const string TASKS_DELETED_PARTIALLY = "{0} tarefa(s) excluída(s). {1} tarefa(s) não encontrada(s).";
}