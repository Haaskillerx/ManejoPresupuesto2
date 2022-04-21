

function inicializarFormulario(obtenerCategorias) {


    const selectElement = document.querySelector('#ID_OPERACION');
    selectElement.addEventListener('change', (event) => {
        onChange_Operacion(); // Funcion Asyncrona
    })

    async function onChange_Operacion() {
        var e = document.getElementById("ID_OPERACION");
        var valorSeleccionado = e.options[e.selectedIndex].value;

        const respuesta = await fetch(url2, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await respuesta.json();
        const opciones =
            json.map(x => `<option value=${x.value}>${x.text}</option>`);
        $("#ID_CATEGORIA").html(opciones);

        console.log(json);
    }



}