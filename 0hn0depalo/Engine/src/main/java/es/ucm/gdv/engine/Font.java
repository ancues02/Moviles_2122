package es.ucm.gdv.engine;

/**
 * Interfaz que representa las fuentes
 * y permite abstraer la implementacion de la plataforma
 */
public interface Font {
    /**
     * Devuelve el nombre del archivo
     * que contiene la imagen
     *
     * @return el nombre del archivo
     */
    String getFilename();

    /**
     * Establece el texto en negrita
     *
     * @param bold true para poner la fuente en negrita
     */
    void setBold(boolean bold);

    /**
     * Establece el tamaño del texto
     *
     * @param size el tamaño del texto
     */
    void setSize(float size);
}
