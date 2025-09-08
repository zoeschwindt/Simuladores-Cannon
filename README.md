# Simuladores-Cannon

Proyecto realizado en Unity para la materia Simuladores.

---

## Cómo jugar
1. Ajustá la **posición del cañón** con la barra horizontal (movimiento).
2. Configurá el **ángulo del cañón** con la barra central.
3. Seleccioná la **velocidad de la bala** con la barra derecha.
4. Elegí la **masa del proyectil** con el botón en pantalla.
5. Presioná la tecla **Espacio** para disparar.

El objetivo es Tirar todas las botellas. 

---

## Versión de Unity
Unity 6.0 (6000.0.47f1)
---

## Controles
- **Barra horizontal (slider)** → movimiento del cañón.  
- **Barra central (slider)** → ángulo del disparo.  
- **Barra derecha (slider)** → velocidad inicial de la bala.  
- **Botón en pantalla** → cambia la masa del proyectil.  
- **Espacio** → dispara el cañón.

---

## Criterios de evaluación
Requisitos mínimos


Controles de disparo en pantalla:
Ángulo y fuerza con Slider o InputField.
Masa del proyectil seleccionable


Disparo físico:
Proyectil con Rigidbody y Collider.
Lanzamiento por AddForce o velocity según el ángulo configurado.


Escena de objetivos:
Estructuras armadas con Rigidbodies y Joints (FixedJoint, HingeJoint o SpringJoint).
Estabilidad inicial correcta. Si se cae sola, está mal configurada.


Registro del resultado:
Guardar datos como tiempo de vuelo, punto de impacto, velocidad relativa, impulso de colisión y piezas derribadas.
Mostrar al final de cada intento: puntuación y un breve “reporte de tiro”.


Entrega


Repositorio en Git:
README.md con cómo jugar, versión de Unity, controles y criterios de evaluación.
Carpeta Assets/ con scripts y prefabs ordenados.
Commits claros. Evitar subir Library/ y builds.


Video en YouTube:
1 a 3 minutos. Mostrar interfaz, 2 o 3 tiros distintos y el registro de resultados.
Link en el README.


---

## Video de demostración
https://youtu.be/m6RDaVcKTtM

---

- Zoe Schwindt
