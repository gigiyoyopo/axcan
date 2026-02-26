let selectedRating = 0;
let reviews = JSON.parse(localStorage.getItem("reviews")) || [];

document.addEventListener("DOMContentLoaded", function () {

    const stars = document.querySelectorAll('#starRating span');

    stars.forEach(star => {
        star.addEventListener('click', function () {
            selectedRating = parseInt(this.dataset.value);
            updateStars();
        });
    });

    renderReviews();
    calculateAverage();
});

function updateStars() {
    const stars = document.querySelectorAll('#starRating span');

    stars.forEach(star => {
        star.classList.toggle(
            'active',
            parseInt(star.dataset.value) <= selectedRating
        );
    });
}

function addReview() {

    const name = document.getElementById("clientName").value.trim();
    const comment = document.getElementById("reviewComment").value.trim();

    if (!name || !comment || selectedRating === 0) {
        alert("Completa todos los campos");
        return;
    }

    const review = {
        name,
        rating: selectedRating,
        comment,
        date: new Date().toLocaleDateString()
    };

    reviews.push(review);
    localStorage.setItem("reviews", JSON.stringify(reviews));

    renderReviews();
    calculateAverage();

    resetForm();
}

function renderReviews() {

    const container = document.getElementById("reviewsContainer");
    const filter = parseInt(document.getElementById("filterStars").value);

    container.innerHTML = "";

    reviews
        .filter(r => filter === 0 || r.rating === filter)
        .forEach(r => {

            const div = document.createElement("div");
            div.classList.add("review-card");

            div.innerHTML = `
                <div class="review-header">
                    <strong>${r.name}</strong>
                    <span class="review-date">${r.date}</span>
                </div>
                <div class="stars">${"★".repeat(r.rating)}</div>
                <p>${r.comment}</p>
            `;

            container.appendChild(div);
        });
}

function calculateAverage() {

    if (reviews.length === 0) return;

    const avg = reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length;

    document.getElementById("averageValue").innerText = avg.toFixed(1);
    document.getElementById("averageStars").innerHTML =
        "★".repeat(Math.round(avg));
    document.getElementById("totalReviews").innerText =
        `(${reviews.length} reseñas)`;
}

function filterReviews() {
    renderReviews();
}

function resetForm() {
    document.getElementById("clientName").value = "";
    document.getElementById("reviewComment").value = "";
    selectedRating = 0;
    updateStars();
}

function reportBusiness() {
    const report = document.getElementById("reportComment").value.trim();

    if (!report) {
        alert("Describe el problema");
        return;
    }

    alert("Reporte enviado correctamente");
    document.getElementById("reportComment").value = "";
}