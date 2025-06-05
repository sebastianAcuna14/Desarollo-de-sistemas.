// Inicializar DataTable
document.addEventListener('DOMContentLoaded', function () {
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10, // 10 filas por página
            labels: {
                placeholder: "Buscar empleado...", // Texto del buscador
                noRows: "No se encontraron empleados", // Mensaje sin datos
                info: "Mostrando {start} a {end} de {rows} empleados", // Texto de paginación
            },
            columns: [
                { select: 0, sort: "asc" }, // Ordenar por ID ascendente al inicio
                { select: 5, type: "date", format: "DD/MM/YYYY" } // Formato para fecha
            ]
        });
    }
});