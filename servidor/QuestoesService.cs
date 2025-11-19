using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace servidor
{
    public class QuestoesService
    {
        public List<Questao> _questoes;
        private readonly string _jsonPath = "banco/questoes.json";
        private int _currentQuestao;

        public QuestoesService()
        {
            _questoes = this.GetQuestoes();
            _currentQuestao = 0;
        }

        public Questao? GetNextQuestao()
        {
            if (_currentQuestao > (_questoes.Count - 1)) return null;

            Questao questao = _questoes[_currentQuestao];

            _currentQuestao++; 
            return questao;
        }

        public List<Questao> GetQuestoes()
        {
            string json = GetJsonQuestoes();

            return DeserializeJsonQuestoes(json);
        }

        private string GetJsonQuestoes() => File.ReadAllText(_jsonPath);

        private List<Questao> DeserializeJsonQuestoes(string json)
        {
            JsonSerializerOptions opcoes = new() { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<List<Questao>>(json, opcoes) ?? [];
        } 
            
    }
}