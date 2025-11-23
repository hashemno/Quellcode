# --------------------------------------------------------------------------- #
# Automatisierungsskript für die Pixyz-Pipeline
# --------------------------------------------------------------------------- #
# Dieses Skript automatisiert die Verarbeitung von CAD-Dateien in Pixyz:
# - Import der CAD-Dateien
# - Tesselierung und Mesh-Reparatur
# - Materialzuweisung
# - Optimierung und Export
# Ziel: Einheitliche und optimierte 3D-Daten für FuQS3D- und CATIA-Prozesse
# --------------------------------------------------------------------------- #

import json    # Zum Einlesen der Konfigurationsdatei (JSON)
import time    # Für Zeitmessung und Performance-Auswertung
import pxz     # Pixyz Python-API für Automatisierungsprozesse

# Initialisierung der Pixyz-Umgebung
pxz.initialize()

# Konfiguration des Lizenzservers
pxz.core.configureLicenseServer(
    "pixyzlic10.wob.vw.vwg;pixyzlic11.wob.vw.vwg;pixyzlic12.wob.vw.vwg",
    28145,
    True
)

def fuqs3dv1():
    """
    Hauptfunktion:
    - Liest die Konfigurationsdatei (JSON)
    - Iteriert über alle Eingabedateien
    - Ruft die Tesselations- und Exportfunktion für jede Datei auf
    """
    with open('C:/Users/P340679/pag_catia/pixyz/fuqs3d_config.json') as config:
        inputs = json.load(config)
        # Pfade und Einstellungen aus der Konfiguration
        input_folder = inputs['input_folder']
        input_files = inputs['input_files']
        output_folder = inputs['output_folder']
        export_file_format = inputs['export_file_format']
        optimization_def = inputs['optimization']
        exception = inputs['exception']
        exceptions = inputs['exceptions']
        prior_change = inputs['prior_change']
        priorities = inputs['priorities']

        # Verarbeitung jeder Datei
        for input_file in input_files:
            tesselationfuqs(
                input_folder, input_file, output_folder, export_file_format,
                optimization_def, exception, exceptions, prior_change, priorities
            )

    print("Alle Dateien exportiert")
    print("Timestamp:", time.time())

def tesselationfuqs(input_folder, input_file, output_folder, export_file_format,
                    optimization_def, exception, exceptions, prior_change, priorities):
    """
    Verarbeitung einer einzelnen CAD-Datei:
    - Import
    - Tesselierung
    - Mesh-Reparatur
    - Materialzuweisung
    - Optimierung
    - Export
    """
    # PMI-Daten beim Import deaktivieren
    pxz.core.setModuleProperty("IO", "LoadPMI", "False")

    # Import der Datei
    print("Importiere Datei:", input_folder + input_file)
    createdOcc = pxz.io.importScene(input_folder + input_file)

    # Tesselierung der Geometrie
    pxz.algo.tessellate([1], 0.2, -1, -1, True, 0, 1, 0, False, False, True, False)

    # Reparatur des Meshs
    pxz.algo.repairMesh([1], 0.1, True, False)

    # Löschen unnötiger Linien und Patches
    pxz.algo.deleteLines([1])
    pxz.algo.deletePatches([1], True)

    # Materialzuweisung
    pxz.scene.clearSelection()
    pxz.scene.select([1])
    pxz.scene.removeMaterials()
    pxz.scene.clearSelection()
    grayMat = pxz.material.createMaterial("Dark Grey", "color")
    pxz.core.setProperty(grayMat, "color", "[0.5, 0.5, 0.5, 1.0]")

    # Statistik vor Verarbeitung
    t0, n_triangles, n_vertices, n_parts = getStatsfuqs(pxz.scene.getRoot())

    # Optimierungen auf Szenenebene
    pxz.scene.deleteEmptyOccurrences(1)
    pxz.scene.mergeOccurrencesByTreeLevel([1], 6, 0)
    pxz.scene.convertToOldSchoolVisibility(root=1)

    # Verarbeitung aller Teile nach Metadaten
    occurrences = pxz.scene.findOccurrencesByMetadata(
        "PartNumber",
        r"^[A-Z]{2}[0-9]{3}[A-Z0-9\-]*_[A-Z0-9]*_[A-Z0-9]*_[A-Z0-9\-]*_[A-Z0-9]*_*",
        []
    )

    for part in occurrences:
        # Material und Sichtbarkeit setzen
        pxz.scene.mergePartOccurrences([part], 0)
        pxz.scene.setOccurrenceMaterial(part, grayMat)
        pxz.scene.convertToOldSchoolVisibility(part)

        # Transformationen auf das Teil anwenden
        pxz.scene.applyTransformation(part, [
            [0, 0, 0.001, 0],
            [0, 0.001, 0, 0],
            [-0.001, 0, 0, 0],
            [0, 0, 0, 1]
        ])
        pxz.scene.resetTransform(part, True, True, False)
        pxz.scene.applyTransformation(part, [
            [1000, 0, 0, 0],
            [0, 1000, 0, 0],
            [0, 0, 1000, 0],
            [0, 0, 0, 1]
        ])

        # Metadaten auslesen
        metadataComponents = pxz.scene.getComponentByOccurrence([part], 5)
        metadataDefinitions = pxz.scene.getMetadatasDefinitions(metadataComponents)
        part_number = ""
        revision = ""
        description = ""
        for item in metadataDefinitions[0]:
            if item.name == 'PartNumber':
                part_number = item.value
            elif item.name == 'Revision':
                revision = item.value
            elif item.name == 'DescriptionRef':
                description = item.value

        # Export vorbereiten
        pxz.scene.clearSelection()
        pxz.scene.select([part])
        output_file = output_folder + part_number + revision[-1:] + "_" + description + export_file_format
        pxz.io.exportSelection(output_file, False)
        pxz.scene.clearSelection()

    # Statistik nach Verarbeitung
    t1, _n_triangles, _n_vertices, _n_parts = getStatsfuqs(pxz.scene.getRoot())
    printStatsfuqs(input_file, t1 - t0, n_triangles, _n_triangles, n_vertices, _n_vertices, n_parts, _n_parts)
    pxz.core.resetSession()

def getStatsfuqs(root):
    """
    Ermittelt Anzahl von Dreiecken, Vertices und Parts einer Szene.
    Gibt zudem den Zeitstempel der Verarbeitung zurück.
    """
    pxz.core.configureInterfaceLogger(False, False, False)  # Temporär Logging aus
    t = time.time()
    n_triangles = pxz.scene.getPolygonCount([root], True, False, False)
    n_vertices = pxz.scene.getVertexCount([root], False, False, False)
    n_parts = len(pxz.scene.getPartOccurrences(root))
    pxz.core.configureInterfaceLogger(True, True, True)  # Logging wieder ein
    return t, n_triangles, n_vertices, n_parts

def printStatsfuqs(input_file, t, n_triangles, _n_triangles, n_vertices, _n_vertices, n_parts, _n_parts):
    """
    Gibt die Statistiken der Datei nach Optimierung aus.
    """
    print('\n')
    print(f"Datei: {input_file}")
    print(f"Optimierungsdauer: {t:.3f} s")
    print(f"Triangles: {n_triangles} -> {_n_triangles}")
    print(f"Vertices: {n_vertices} -> {_n_vertices}")
    print(f"Parts: {n_parts} -> {_n_parts}")

# Einstiegspunkt des Skripts
if __name__ == "__main__":
    fuqs3dv1()
