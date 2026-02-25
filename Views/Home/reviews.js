let selectedRating = 0;
let reviews = [];

document.addEventListener("DOMContentLoaded", function () {

    const stars = document.querySelectorAll('#starRating span');

    stars.forEach(star => {
        star.addEventListener('click', function () {
            selectedRating = parseInt(this.getAttribute('data-value'));
            updateStars();
        });
    });

});

function updateStars() {
    const stars = document.querySelectorAll('#starRating span');

    stars.forEach(star => {
        star.classList.remove('active');
        if (parseInt(star.getAttribute('data-value')) <= selectedRating) {
            star.classList.add('active');
        }
    });
}

function addReview() {
    const name = document.getElementById("clientName").value;
    const comment = document.getElementById("comment").value;

    if (!name || !comment || selectedRating === 0) {
        alert("Completa todos los campos");
        return;
    }

    const review = {
        name,
        rating: selectedRating,
        comment
    };

    reviews.push(review);

    renderReviews();
    calculateAverage();

    document.getElementById("clientName").value = "";
    document.getElementById("comment").value = "";
    selectedRating = 0;
    updateStars();
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
                <strong>${r.name}</strong>
                <div class="stars">${"★".repeat(r.rating)}</div>
                <p>${r.comment}</p>
            `;

            container.appendChild(div);
        });
}

function calculateAverage() {
    const avg = reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length;
    document.getElementById("averageValue").innerText = avg.toFixed(1);
    document.getElementById("averageStars").innerHTML = "★".repeat(Math.round(avg));
    document.getElementById("totalReviews").innerText = `(${reviews.length} reseñas)`;
}

function filterReviews() {
    renderReviews();
}
