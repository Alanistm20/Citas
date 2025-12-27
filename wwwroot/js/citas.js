function reservar() {
    const token = document.querySelector('meta[name="csrf-token"]')?.getAttribute('content');

    fetch("/Citas/Reservar", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "RequestVerificationToken": token } : {})
        },
        body: JSON.stringify({
            idMedico: document.getElementById("medico").value,
            fecha: document.getElementById("fecha").value,
            hora: document.getElementById("hora").value,
            motivo: document.getElementById("motivo").value
        })
    })
        .then(r => r.json())
        .then(r => {
            let msg = document.getElementById("msg");
            msg.innerHTML = r.ok
                ? `<div class="alert alert-success">Cita reservada</div>`
                : `<div class="alert alert-danger">${r.mensaje}</div>`;
        });
}
