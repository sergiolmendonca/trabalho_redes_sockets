namespace cliente
{
    public enum TiposResposta
    {
        NICKNAME = 0,
        PERGUNTA = 1,
        RESPOSTA = 2,
        MENSAGEM = 3,
        RESULTADO = 4
    }

    public enum StatusResposta
    {
        OK = 0,
        ERRO = 1
    }

    public enum TipoResultado
    {
        PARCIAL = 0,
        FINAL = 1
    }

    public enum StatusResultado
    {
        ERROU = 0,
        ACERTOU_ANTES = 1,
        ACERTOU_DEPOIS = 2,
        ACERTOU_SEM_PONTUACAO = 3
    }
}