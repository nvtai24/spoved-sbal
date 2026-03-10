DROP TABLE IF EXISTS fruits;

CREATE TABLE fruits (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    image_url TEXT,
    sweetness INT CHECK (sweetness BETWEEN 1 AND 10),
    created_at TIMESTAMP DEFAULT NOW()
);

-- Seed sample data
INSERT INTO fruits (name, image_url, sweetness) VALUES
('Apple', 'https://upload.wikimedia.org/wikipedia/commons/1/15/Red_Apple.jpg', 7),
('Banana', 'https://upload.wikimedia.org/wikipedia/commons/8/8a/Banana-Single.jpg', 9),
('Orange', 'https://upload.wikimedia.org/wikipedia/commons/c/c4/Orange-Fruit-Pieces.jpg', 6),
('Strawberry', 'https://upload.wikimedia.org/wikipedia/commons/2/29/PerfectStrawberry.jpg', 8),
('Mango', 'https://upload.wikimedia.org/wikipedia/commons/9/90/Hapus_Mango.jpg', 10),
('Pineapple', 'https://upload.wikimedia.org/wikipedia/commons/c/cb/Pineapple_and_cross_section.jpg', 7),
('Watermelon', 'https://upload.wikimedia.org/wikipedia/commons/e/ee/Watermelon_cross_BNC.jpg', 8),
('Grape', 'https://upload.wikimedia.org/wikipedia/commons/1/1b/Table_grapes_on_white.jpg', 7),
('Lemon', 'https://upload.wikimedia.org/wikipedia/commons/c/c8/Lemon.jpg', 2),
('Cherry', 'https://upload.wikimedia.org/wikipedia/commons/b/bb/Cherry_Stella444.jpg', 8),
('Peach', 'https://upload.wikimedia.org/wikipedia/commons/9/9f/Peach_and_cross_section.jpg', 7),
('Kiwi', 'https://upload.wikimedia.org/wikipedia/commons/d/d3/Kiwi_aka.jpg', 6),
('Papaya', 'https://upload.wikimedia.org/wikipedia/commons/6/6b/Papaya_cross_section.jpg', 9),
('Guava', 'https://upload.wikimedia.org/wikipedia/commons/0/02/Guava_ID.jpg', 6),
('Dragon Fruit', 'https://upload.wikimedia.org/wikipedia/commons/5/56/Pitaya_cross_section_ed2.jpg', 5);

-- Test
SELECT * FROM fruits;