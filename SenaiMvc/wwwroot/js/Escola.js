var escola = function () {
    return {
        carregarCidadesPorUf: function (uf) {
            if (!uf) return;
            var select = document.getElementById("Endereco_Cidade");
            select.innerHTML = '<option value="">Carregando...</option>';

            fetch(`/Escola/ObterCidadesPorUF?uf=${uf}`)
                .then(response => response.json())
                .then(cidades => {
                    select.innerHTML = '<option value="">-- Selecione a cidade --</option>';

                    cidades.forEach(function (cidade) {
                        var option = document.createElement("option");
                        option.value = cidade.id;
                        option.text = cidade.nome;
                        select.appendChild(option);
                    });
                })
                .catch(err => {
                    console.error("Erro ao carregar cidades:", err);
                });
        }
    }
}();