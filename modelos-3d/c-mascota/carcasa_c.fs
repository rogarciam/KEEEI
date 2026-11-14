FeatureScript 3112;
import(path : "onshape/std/common.fs", version : "3112.0");

/*
 * KEEEI · Carcasa C «Keko, el robotito tragallaves»
 * Mascota de sobremesa: un robotito-hucha. Las llaves se le echan por la ranura de la cabeza
 * (como a una hucha), los 2 LED son sus ojos, el QR va en la barriga y unas orejas-antena lo
 * rematan. Se queda de pie sobre una peana ancha con dos pies, lastrable con tuercas M8.
 *
 * Todo lo funcional sale de la carcasa A (modelos-3d/a-ranura-vertical): cartucho del mecanismo con
 * empujador de leva y microswitch KW12-3, protoboard de 40 x 50 con el ESP32, paso USB-C, insertos M3
 * y holguras con los MISMOS parámetros y valores por defecto (la calibración de la A vale aquí).
 *
 * Ejes:  X = ancho · Y = profundidad (Y=0 tapa trasera, Y=D cara frontal) · Z = altura (Z=0 base del cuerpo)
 * Piezas (todas sin soportes; orientación de impresión decidida antes de modelar):
 *   "Cuerpo"             azul. Cara frontal sobre la cama (como la A).
 *   "Cartucho mecanismo" tumbado sobre su costado -X (como la A).
 *   "Empujador"          de pie sobre la cara trasera del tope (como la A).
 *   "Tapa trasera"       azul. Cara exterior sobre la cama. 4 tornillos M3 a insertos (sin ojales de pared).
 *   "Cara"               blanca. Visor con los dos ojos (anillos con el LED de pupila) y la sonrisa grabada.
 *                        Cara trasera sobre la cama; 2 pasadores de filamento de 1,75 mm.
 *   "Barriga"            blanca. Placa con el rebaje del QR. Cara trasera sobre la cama; 2 pasadores.
 *   "Oreja izquierda" / "Oreja derecha"  blancas. Disco con antena, plano: cara interior sobre la cama; 2 pasadores.
 *   "Peana"              azul o blanca. Base con dos pies, canal del cable USB y 2 alojamientos para tuercas M8.
 *                        Cara inferior sobre la cama; el cuerpo se pega encima centrado con 2 pasadores.
 * Referencia de impresión: PETG, boquilla 0,4 mm, capa 0,2 mm.
 */

// ---------------- Constantes internas (no expuestas en la UI) ----------------
const ZERO         = 0 * millimeter;
const EPS          = 0.2 * millimeter;  // solape para uniones booleanas
const FENCE_T      = 1.2 * millimeter;  // grosor de las escuadras que centran la protoboard
const FENCE_CLR    = 0.3 * millimeter;  // holgura escuadra - pared interior de la carcasa
const FENCE_IN     = 6 * millimeter;    // tamaño de la escuadra/soporte hacia dentro de la placa
const POST_INSET   = 3 * millimeter;    // centro de los postes de apriete desde el canto de la placa
const POST_HALF    = 1.5 * millimeter;  // medio lado de los postes (3 x 3 mm)
const SLEEVE_T     = 1.6 * millimeter;  // pared de la camisa de la ranura (4 líneas: con los chaflanes quedan ≥ 0,8 mm)
const SHELF_T      = 1.6 * millimeter;  // balda del microswitch
const SLOT_CHAMFER = 0.5 * millimeter;  // chaflán de entrada de la ranura
const BOSS_SINK    = 0.6 * millimeter;  // los tetones se hunden en las paredes (evita contactos tangentes)
const M2_CLEAR     = 2.4 * millimeter;  // paso del tornillo M2 en la balda del microswitch
const M2_PILOT     = 1.7 * millimeter;  // agujero piloto M2 (autorroscante) pasante en la balda superior: vale un M2 x 10
const RIB_T        = 1.2 * millimeter;  // nervio que arriostra cada poste de apriete contra la pared lateral
const GUIDE_T      = 1.2 * millimeter;  // pared de la guía del empujador
const COLLAR_X     = 2 * millimeter;    // el tope del empujador sobresale esto a cada lado (X)
const COLLAR_T     = 2 * millimeter;    // grosor del tope (Y); el rodillo apoya en su cara trasera
const PUSH_FLAT    = 0.5 * millimeter;  // parte recta mínima de la punta entre las dos rampas (apoya el llavero)
const PUSH_ROUND   = 0.8 * millimeter;  // redondeo de las aristas donde las rampas nacen del vástago
const PUSH_BED_CH  = 0.3 * millimeter;  // chaflán de las aristas del empujador que apoyan en la cama
const COUPON_CH    = 0.4 * millimeter;  // chaflán de las aristas de corte de las piezas de prueba que apoyan en la cama
const PETG_DENSITY = 1.27;              // g/cm³, para estimar el material de las piezas de prueba
const MIN_FLOOR    = 0.8 * millimeter;  // suelo/techo mínimo: 4 capas de 0,2 mm
const INLAY_T      = 1.6 * millimeter;  // grosor del frontal pegado (8 capas)
const PIN_D        = 1.9 * millimeter;  // agujero de los pasadores de centrado (trozos de filamento de 1,75 mm)
const PIN_IN_BODY  = 1.0 * millimeter;  // profundidad del agujero del pasador en la pared frontal
const PIN_IN_INLAY = 0.8 * millimeter;  // profundidad del agujero del pasador en el frontal
const CAP_GAP      = 0.2 * millimeter;  // la franja de techo del cartucho no llega a la tapa: cadena de topes no hiperestática
const SAC_LAYER    = 0.2 * millimeter;  // capa de sacrificio sobre los avellanados de la tapa (se perfora al montar)
const INLAY_BORDER = 1.6 * millimeter;  // margen del frontal alrededor del QR y de los LED
const RIB_SUP_T    = 1.6 * millimeter;  // nervio de la pared frontal sobre el que apoya el cartucho
const RIB_SUP_D    = 3 * millimeter;    // cuánto entra ese nervio hacia atrás desde la pared frontal
const CART_BED_CH  = 0.3 * millimeter;  // chaflán de las aristas del cartucho que apoyan en la cama
// --- Mascota (carcasa C) ---
const EYE_R        = 4.5 * millimeter;  // radio exterior del anillo de cada ojo
const EYE_RING_H   = 1.2 * millimeter;  // lo que sobresale el anillo del ojo sobre el visor (6 capas)
const VISOR_BORDER = 1.2 * millimeter;  // margen del visor alrededor de los anillos de los ojos
const LED_FLANGE_R = 1.9 * millimeter;  // radio de la pestaña del LED de 3 mm (por dentro de la pared frontal)
const SMILE_R      = 4.2 * millimeter;  // radio exterior de la sonrisa grabada
const SMILE_W      = 1.2 * millimeter;  // ancho del trazo de la sonrisa (3 líneas)
const SMILE_DEPTH  = 0.4 * millimeter;  // profundidad de la sonrisa (2 capas)
const VISOR_PIN_DX = 9 * millimeter;    // pasadores del visor: separación desde el centro
const VISOR_PIN_DZ = 2.5 * millimeter;  // pasadores del visor: altura sobre los ojos
const EAR_T        = 2.4 * millimeter;  // grosor de la oreja (12 capas)
const EAR_KNOB_H   = 1.2 * millimeter;  // botón decorativo de la oreja
const EAR_KNOB_IN  = 2.5 * millimeter;  // el botón queda esto por dentro del borde del disco
const EAR_PIN_DY   = 4 * millimeter;    // pasadores de la oreja: separación desde su centro
const ANT_W        = 3 * millimeter;    // ancho del palo de la antena
const BASE_FLOOR   = 1.2 * millimeter;  // suelo de la peana bajo el canal y los alojamientos (6 capas)
const BASE_WALL    = 1.2 * millimeter;  // tabique mínimo entre huecos de la peana
const BASE_TOP_R   = 2 * millimeter;    // redondeo del canto superior de la peana
const BASE_BED_CH  = 0.5 * millimeter;  // chaflán de la peana en la cama
const BASE_PIN_IN  = 1.0 * millimeter;  // profundidad del pasador en la peana (pasador de 1,8 mm en total)
const BASE_PIN_DX  = 12 * millimeter;   // pasadores peana-cuerpo: separación desde el centro
const BASE_PIN_DY  = 2.5 * millimeter;  // pasadores peana-cuerpo: distancia a la cara frontal
const TOE_GAP      = 4 * millimeter;    // separación entre los dos pies
const CHAN_EXTRA   = 0.75 * millimeter; // el canal del cable es esto más ancho por lado que el paso USB

// ---------------- Límites de parámetros [mín, defecto, máx] en mm ----------------
const B_WALL      = { (millimeter) : [1.2, 2.0, 5.0] } as LengthBoundSpec;
const B_RADIUS    = { (millimeter) : [1.0, 6.0, 20.0] } as LengthBoundSpec;
const B_CHAMFER_F = { (millimeter) : [0.2, 1.0, 3.0] } as LengthBoundSpec;
const B_CHAMFER_B = { (millimeter) : [0.2, 0.5, 2.0] } as LengthBoundSpec;
const B_PLUG      = { (millimeter) : [5.0, 10.0, 40.0] } as LengthBoundSpec;
const B_PCB_W     = { (millimeter) : [26.0, 40.0, 80.0] } as LengthBoundSpec;
const B_PCB_H     = { (millimeter) : [49.0, 50.0, 100.0] } as LengthBoundSpec;
const B_PCB_T     = { (millimeter) : [0.8, 1.6, 2.4] } as LengthBoundSpec;
const B_PCB_Y     = { (millimeter) : [2.0, 4.0, 10.0] } as LengthBoundSpec;
const B_POST_GAP  = { (millimeter) : [-0.3, 0.0, 1.0] } as LengthBoundSpec;
const B_HEADER    = { (millimeter) : [0.0, 8.5, 15.0] } as LengthBoundSpec;
const B_ESP_FRONT = { (millimeter) : [3.0, 4.8, 10.0] } as LengthBoundSpec;
const B_USB_C     = { (millimeter) : [1.0, 3.15, 6.0] } as LengthBoundSpec;
const B_USB_OW    = { (millimeter) : [9.5, 13.5, 20.0] } as LengthBoundSpec;
const B_USB_OT    = { (millimeter) : [4.0, 8.0, 14.0] } as LengthBoundSpec;
const B_ESP_OFF   = { (millimeter) : [-20.0, 0.0, 20.0] } as LengthBoundSpec;
const B_TAG_W     = { (millimeter) : [10.0, 22.0, 40.0] } as LengthBoundSpec;
const B_TAG_T     = { (millimeter) : [1.0, 3.0, 6.0] } as LengthBoundSpec;
const B_SLOT_D    = { (millimeter) : [10.0, 28.0, 45.0] } as LengthBoundSpec;
const B_FIT_SLIDE = { (millimeter) : [0.1, 0.25, 0.6] } as LengthBoundSpec;
const B_FIT_FIRM  = { (millimeter) : [0.0, 0.15, 0.4] } as LengthBoundSpec;
const B_FIT_SW    = { (millimeter) : [0.0, 0.1, 0.4] } as LengthBoundSpec;
const B_SW_L      = { (millimeter) : [10.0, 20.0, 30.0] } as LengthBoundSpec;
const B_SW_T      = { (millimeter) : [3.0, 6.0, 12.0] } as LengthBoundSpec;
const B_SW_H      = { (millimeter) : [4.0, 8.4, 15.0] } as LengthBoundSpec;
const B_SW_PRESS  = { (millimeter) : [5.0, 12.2, 25.0] } as LengthBoundSpec;
const B_SW_REST   = { (millimeter) : [5.0, 20.0, 30.0] } as LengthBoundSpec;
const B_SW_OP     = { (millimeter) : [5.0, 17.0, 30.0] } as LengthBoundSpec;
const B_REST_M    = { (millimeter) : [0.3, 1.5, 4.0] } as LengthBoundSpec;
const B_OVERTRAV  = { (millimeter) : [0.2, 0.8, 4.0] } as LengthBoundSpec;
const B_SW_RESID  = { (millimeter) : [0.0, 0.3, 1.5] } as LengthBoundSpec;
const B_PUSH_Z    = { (millimeter) : [3.0, 9.0, 25.0] } as LengthBoundSpec;
const B_PUSH_W    = { (millimeter) : [3.0, 8.0, 20.0] } as LengthBoundSpec;
const B_FIT_GUIDE = { (millimeter) : [0.1, 0.25, 0.6] } as LengthBoundSpec;
const B_GUIDE_L   = { (millimeter) : [2.0, 4.0, 12.0] } as LengthBoundSpec;
const B_ROLLER_X  = { (millimeter) : [-10.0, 0.0, 10.0] } as LengthBoundSpec;
const B_RAMP_IN   = { (degree) : [20, 45, 55] } as AngleBoundSpec;
const B_RAMP_OUT  = { (degree) : [20, 52, 65] } as AngleBoundSpec;
const B_CONN      = { (millimeter) : [5.0, 11.0, 25.0] } as LengthBoundSpec;
const B_HOLE_S    = { (millimeter) : [3.0, 9.5, 20.0] } as LengthBoundSpec;
const B_HOLE_Y    = { (millimeter) : [1.0, 2.5, 10.0] } as LengthBoundSpec;
const B_BACK_T    = { (millimeter) : [2.0, 3.0, 6.0] } as LengthBoundSpec;
const B_INSERT_D  = { (millimeter) : [2.0, 4.0, 6.0] } as LengthBoundSpec;
const B_INSERT_L  = { (millimeter) : [2.0, 6.0, 12.0] } as LengthBoundSpec;
const B_BOSS_R    = { (millimeter) : [2.0, 3.5, 6.0] } as LengthBoundSpec;
const B_SCREW_D   = { (millimeter) : [2.0, 3.4, 5.0] } as LengthBoundSpec;
const B_CB_D      = { (millimeter) : [4.0, 6.5, 10.0] } as LengthBoundSpec;
const B_CB_DEPTH  = { (millimeter) : [0.0, 1.7, 4.0] } as LengthBoundSpec;
const B_QR_SIZE   = { (millimeter) : [10.0, 30.0, 60.0] } as LengthBoundSpec;
const B_QR_FIT    = { (millimeter) : [0.0, 0.2, 1.0] } as LengthBoundSpec;
const B_QR_DEPTH  = { (millimeter) : [0.2, 0.4, 1.2] } as LengthBoundSpec;
const B_QR_R      = { (millimeter) : [0.5, 2.0, 6.0] } as LengthBoundSpec;
const B_QR_Z      = { (millimeter) : [10.0, 24.0, 200.0] } as LengthBoundSpec;
const B_LED_D     = { (millimeter) : [1.5, 3.1, 6.0] } as LengthBoundSpec;
const B_LED_S     = { (millimeter) : [4.0, 32.0, 40.0] } as LengthBoundSpec;
const B_LED_Z     = { (millimeter) : [5.0, 72.0, 200.0] } as LengthBoundSpec;
const B_EAR_R     = { (millimeter) : [5.0, 7.0, 12.0] } as LengthBoundSpec;
const B_EAR_Z     = { (millimeter) : [10.0, 72.0, 200.0] } as LengthBoundSpec;
const B_ANT_H     = { (millimeter) : [0.0, 9.0, 30.0] } as LengthBoundSpec;
const B_ANT_BALL  = { (millimeter) : [2.0, 3.5, 6.0] } as LengthBoundSpec;
const B_BASE_H    = { (millimeter) : [8.0, 15.0, 30.0] } as LengthBoundSpec;
const B_BASE_MS   = { (millimeter) : [0.0, 6.0, 20.0] } as LengthBoundSpec;
const B_BASE_MF   = { (millimeter) : [0.0, 6.0, 30.0] } as LengthBoundSpec;
const B_BASE_MB   = { (millimeter) : [0.0, 6.0, 20.0] } as LengthBoundSpec;
const B_BASE_R    = { (millimeter) : [2.0, 10.0, 25.0] } as LengthBoundSpec;
const B_TOE_R     = { (millimeter) : [0.0, 10.0, 20.0] } as LengthBoundSpec;
const B_NUT_AF    = { (millimeter) : [5.0, 13.0, 17.0] } as LengthBoundSpec;
const B_EXPLODE   = { (millimeter) : [0.0, 0.0, 100.0] } as LengthBoundSpec;

// Las etiquetas "Name" de los parámetros solo admiten ASCII imprimible: sin tildes, ñ ni °
annotation { "Feature Type Name" : "Carcasa KEEEI C (mascota)",
             "Feature Type Description" : "Keko, el robotito tragallaves: carcasa de sobremesa con el mecanismo de la carcasa A, ojos LED, QR en la barriga, orejas-antena y peana lastrable." }
export const carcasaKeeeiC = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Group Name" : "Carcasa", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Pared lateral / superior / inferior" }
            isLength(definition.wall, B_WALL);
            annotation { "Name" : "Pared frontal" }
            isLength(definition.wallF, B_WALL);
            annotation { "Name" : "Radio de esquinas (vista frontal)" }
            isLength(definition.cornerR, B_RADIUS);
            annotation { "Name" : "Bisel frontal 45 grados" }
            isLength(definition.chamferF, B_CHAMFER_F);
            annotation { "Name" : "Bisel trasero 45 grados" }
            isLength(definition.chamferB, B_CHAMFER_B);
            annotation { "Name" : "Hueco para la clavija USB bajo la placa" }
            isLength(definition.plugClear, B_PLUG);
        }
        annotation { "Group Name" : "Protoboard y ESP32", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Protoboard: ancho (X)" }
            isLength(definition.pcbW, B_PCB_W);
            annotation { "Name" : "Protoboard: alto (Z)" }
            isLength(definition.pcbH, B_PCB_H);
            annotation { "Name" : "Protoboard: grosor" }
            isLength(definition.pcbT, B_PCB_T);
            annotation { "Name" : "Altura de los soportes (hueco para soldaduras)" }
            isLength(definition.pcbY, B_PCB_Y);
            annotation { "Name" : "Holgura de los postes de apriete (negativo = presion)" }
            isLength(definition.postGap, B_POST_GAP);
            annotation { "Name" : "Altura de los pines hembra" }
            isLength(definition.headerH, B_HEADER);
            annotation { "Name" : "ESP32: altura del componente mas alto sobre la cara inferior de su PCB" }
            isLength(definition.espFront, B_ESP_FRONT);
            annotation { "Name" : "ESP32: eje del USB sobre la cara inferior de su PCB" }
            isLength(definition.usbC, B_USB_C);
            annotation { "Name" : "ESP32: desplazamiento lateral del USB respecto al centro" }
            isLength(definition.espOffX, B_ESP_OFF);
            annotation { "Name" : "Abertura de la clavija USB: ancho" }
            isLength(definition.usbOW, B_USB_OW);
            annotation { "Name" : "Abertura de la clavija USB: fondo" }
            isLength(definition.usbOT, B_USB_OT);
        }
        annotation { "Group Name" : "Llavero y ranura", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Llavero: ancho" }
            isLength(definition.tagW, B_TAG_W);
            annotation { "Name" : "Llavero: grosor" }
            isLength(definition.tagT, B_TAG_T);
            annotation { "Name" : "Profundidad de la ranura" }
            isLength(definition.slotDepth, B_SLOT_D);
            annotation { "Name" : "Holgura deslizante de la ranura (por lado)" }
            isLength(definition.fitSlide, B_FIT_SLIDE);
            annotation { "Name" : "Holgura de ajuste firme (protoboard, por lado)" }
            isLength(definition.fitFirm, B_FIT_FIRM);
        }
        annotation { "Group Name" : "Microswitch KW12-3", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Cuerpo: largo (X)" }
            isLength(definition.swL, B_SW_L);
            annotation { "Name" : "Rodillo: desplazamiento desde el centro del cuerpo (X)" }
            isLength(definition.rollerX, B_ROLLER_X);
            annotation { "Name" : "Cuerpo: grosor (Z)" }
            isLength(definition.swT, B_SW_T);
            annotation { "Name" : "Cuerpo: alto sin palanca (Y)" }
            isLength(definition.swH, B_SW_H);
            annotation { "Name" : "Alto total con palanca PULSADA" }
            isLength(definition.hPressed, B_SW_PRESS);
            annotation { "Name" : "Alto total con palanca EN REPOSO" }
            isLength(definition.hRest, B_SW_REST);
            annotation { "Name" : "Alto total con la palanca en el punto de clic" }
            isLength(definition.hOP, B_SW_OP);
            annotation { "Name" : "Margen antes del tope (sin aplastar la palanca)" }
            isLength(definition.resid, B_SW_RESID);
            annotation { "Name" : "Holgura del cuerpo entre balda y techo (por lado)" }
            isLength(definition.fitSw, B_FIT_SW);
            annotation { "Name" : "Espacio detras para conector 2x1 y cables" }
            isLength(definition.connSpace, B_CONN);
            annotation { "Name" : "Fijar con 2 tornillos M2 por sus agujeros de montaje", "Default" : true }
            definition.swScrews is boolean;
            if (definition.swScrews)
            {
                annotation { "Name" : "Agujeros de montaje: distancia entre centros" }
                isLength(definition.holeS, B_HOLE_S);
                annotation { "Name" : "Agujeros de montaje: distancia desde la cara de las patillas" }
                isLength(definition.holeY, B_HOLE_Y);
            }
        }
        annotation { "Group Name" : "Empujador", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Altura del empujador sobre el fondo de la ranura" }
            isLength(definition.pushZ, B_PUSH_Z);
            annotation { "Name" : "Margen en reposo por encima del clic" }
            isLength(definition.restMargin, B_REST_M);
            annotation { "Name" : "Recorrido tras el clic con el llavero dentro" }
            isLength(definition.overTravel, B_OVERTRAV);
            annotation { "Name" : "Empujador: ancho (X)" }
            isLength(definition.pushW, B_PUSH_W);
            annotation { "Name" : "Empujador: largo de la guia" }
            isLength(definition.guideL, B_GUIDE_L);
            annotation { "Name" : "Holgura del empujador en su guia (por lado)" }
            isLength(definition.fitGuide, B_FIT_GUIDE);
            annotation { "Name" : "Rampa de entrada (arriba): angulo desde la vertical" }
            isAngle(definition.rampIn, B_RAMP_IN);
            annotation { "Name" : "Rampa de salida (abajo): angulo desde la vertical" }
            isAngle(definition.rampOut, B_RAMP_OUT);
        }
        annotation { "Group Name" : "Tapa trasera e insertos", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Grosor de la tapa trasera" }
            isLength(definition.backT, B_BACK_T);
            annotation { "Name" : "Inserto M3: diametro del agujero" }
            isLength(definition.insertD, B_INSERT_D);
            annotation { "Name" : "Inserto M3: profundidad del agujero" }
            isLength(definition.insertL, B_INSERT_L);
            annotation { "Name" : "Radio de los tetones de los insertos" }
            isLength(definition.bossR, B_BOSS_R);
            annotation { "Name" : "Tornillo: diametro de paso" }
            isLength(definition.screwD, B_SCREW_D);
            annotation { "Name" : "Avellanado plano: diametro" }
            isLength(definition.cbD, B_CB_D);
            annotation { "Name" : "Avellanado plano: profundidad" }
            isLength(definition.cbDepth, B_CB_DEPTH);
        }
        annotation { "Group Name" : "Barriga (etiqueta QR)", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Etiqueta QR: lado" }
            isLength(definition.qrSize, B_QR_SIZE);
            annotation { "Name" : "Etiqueta QR: holgura" }
            isLength(definition.qrFit, B_QR_FIT);
            annotation { "Name" : "Etiqueta QR: profundidad del rebaje" }
            isLength(definition.qrDepth, B_QR_DEPTH);
            annotation { "Name" : "Etiqueta QR: radio de esquinas" }
            isLength(definition.qrR, B_QR_R);
            annotation { "Name" : "Etiqueta QR: altura del centro" }
            isLength(definition.qrZ, B_QR_Z);
        }
        annotation { "Group Name" : "Mascota: ojos (LED) y orejas", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Ojos LED: diametro del agujero" }
            isLength(definition.ledD, B_LED_D);
            annotation { "Name" : "Ojos LED: separacion entre centros" }
            isLength(definition.ledS, B_LED_S);
            annotation { "Name" : "Ojos LED: altura" }
            isLength(definition.ledZ, B_LED_Z);
            annotation { "Name" : "Oreja: radio del disco" }
            isLength(definition.earR, B_EAR_R);
            annotation { "Name" : "Oreja: altura del centro" }
            isLength(definition.earZ, B_EAR_Z);
            annotation { "Name" : "Antena: altura sobre la cabeza" }
            isLength(definition.antH, B_ANT_H);
            annotation { "Name" : "Antena: radio de la bola" }
            isLength(definition.antBall, B_ANT_BALL);
        }
        annotation { "Group Name" : "Peana", "Collapsed By Default" : true }
        {
            annotation { "Name" : "Peana: alto" }
            isLength(definition.baseH, B_BASE_H);
            annotation { "Name" : "Peana: margen lateral" }
            isLength(definition.baseMS, B_BASE_MS);
            annotation { "Name" : "Peana: margen delantero" }
            isLength(definition.baseMF, B_BASE_MF);
            annotation { "Name" : "Peana: margen trasero" }
            isLength(definition.baseMB, B_BASE_MB);
            annotation { "Name" : "Peana: radio de las esquinas" }
            isLength(definition.baseR, B_BASE_R);
            annotation { "Name" : "Pies: radio (0 = sin pies)" }
            isLength(definition.toeR, B_TOE_R);
            annotation { "Name" : "Lastre: entrecaras de la tuerca (M8 = 13 mm)" }
            isLength(definition.nutAF, B_NUT_AF);
        }
        annotation { "Name" : "Separar las piezas (vista explosionada)" }
        isLength(definition.explode, B_EXPLODE);
        annotation { "Name" : "Piezas de prueba (recortes ligeros para ajustar holguras)", "Default" : false }
        definition.testMode is boolean;
    }
    {
        // ====================== 1. COTAS DERIVADAS ======================
        const wall = definition.wall;
        const wallF = definition.wallF;
        const slotHalf = definition.tagW / 2 + definition.fitSlide;          // medio ancho interior de la ranura
        const slotW = definition.tagT + 2 * definition.fitSlide;             // grosor interior de la ranura
        const sleeveHalf = slotHalf + SLEEVE_T;

        const W = max(definition.pcbW + 2 * (definition.fitFirm + FENCE_T + FENCE_CLR + wall),
                      2 * (wall + 2 * definition.bossR) + 2 * sleeveHalf + 1 * millimeter);
        const cx = W / 2;

        // Empujador: el tope fija la palanca en reposo a (clic + margen); el llavero lo hace retroceder
        // hasta (clic - recorrido). Cuánto asoma en la ranura sale de ese recorrido más el hueco llavero-ranura.
        const leverRest = definition.hOP + definition.restMargin;           // alto de la palanca en reposo (contra el tope)
        const leverIn = definition.hOP - definition.overTravel;             // alto de la palanca con el llavero dentro
        const pushTravel = leverRest - leverIn;                             // retroceso del empujador
        const pushOut = pushTravel + (slotW - definition.tagT);             // cuánto asoma la punta en la ranura
        const hP = definition.swT + 2 * definition.fitSw - 2 * definition.fitGuide;  // alto (Z) del empujador: cabe entre baldas
        // Punta en cuña doble: rampa de entrada arriba (la pisa el llavero al bajar) y de salida abajo
        // (evita que el borde de la ventana del llavero se enganche al sacarlo)
        const rampZin = pushOut / tan(definition.rampIn);                   // altura de la rampa de entrada
        const rampZout = pushOut / tan(definition.rampOut);                 // altura de la rampa de salida
        const tipFlat = hP - rampZin - rampZout;                            // parte recta de la punta

        // Fondo: cadena conector -> microswitch -> tope -> guía -> camisa -> ranura -> pared frontal,
        // o la pila protoboard + ESP32 (el switch queda por encima de la electrónica)
        // (el cartucho cierra la ranura por delante con su propia pared: SLEEVE_T más de fondo)
        const D1 = definition.connSpace + leverRest + COLLAR_T + definition.guideL + SLEEVE_T + slotW + SLEEVE_T + wallF;
        const espFrontY = definition.pcbY + definition.pcbT + definition.headerH + definition.espFront;
        const D2 = espFrontY + 1 * millimeter + SLEEVE_T + slotW + SLEEVE_T + wallF;
        const D = max(D1, D2);
        const ysF = D - wallF;                                               // cara interior de la pared frontal
        const ysS = ysF - SLEEVE_T;                                          // cara delantera de la ranura (pared del cartucho)
        const ysB = ysS - slotW;                                             // cara trasera de la ranura
        const yGuideB = ysB - SLEEVE_T - definition.guideL;                  // cara trasera de la guía (apoya el tope)
        const yCollarB = yGuideB - COLLAR_T;                                 // cara trasera del tope (apoya el rodillo)
        const yTip = ysB + pushOut;                                          // punta del empujador en reposo
        const yb = yCollarB - leverRest;                                     // cara de patillas del microswitch

        // Altura: hueco de clavija + protoboard + microswitch (encima de la electrónica) + ranura por encima
        const zB0 = wall + definition.plugClear;
        const zB1 = zB0 + definition.pcbH;
        const fenceTop = zB1 + definition.fitFirm + FENCE_T;
        const shelfTop = fenceTop + 1 * millimeter + SHELF_T;               // cara superior de la balda inferior
        const shelf2Bot = shelfTop + definition.swT + 2 * definition.fitSw; // cara inferior de la balda superior
        const zc = shelfTop + definition.fitSw + definition.swT / 2;        // eje del rodillo y del empujador
        const zSlotBottom = zc - definition.pushZ;
        const H = max(zSlotBottom + definition.slotDepth + wall,
                      shelf2Bot + SHELF_T + 1 * millimeter + wall);
        if (zSlotBottom + definition.slotDepth + wall < H - 0.01 * millimeter)
            throw regenError("La ranura es demasiado corta: no llega desde el techo hasta el empujador. Aumenta su profundidad o reduce la altura del empujador sobre el fondo.");
        const swX = cx - definition.rollerX;                                // centro del cuerpo del switch: su rodillo queda frente al empujador
        // Paso de la clavija USB-C por la base
        const usbX = cx + definition.espOffX;
        const usbY = definition.pcbY + definition.pcbT + definition.headerH + definition.usbC;
        const bIn = wall + definition.bossR - BOSS_SINK;                    // centro de los tetones de las esquinas
        // Barriga pegada (el frontal de la A sin los LED): rodea el QR con un margen INLAY_BORDER
        const qrHalf = definition.qrSize / 2 + definition.qrFit;
        const inlayHalf = qrHalf + INLAY_BORDER;                             // medio ancho de la barriga
        const inlayZ0 = definition.qrZ - qrHalf - INLAY_BORDER;
        const inlayZ1 = definition.qrZ + qrHalf + PIN_D + 2 * MIN_FLOOR + CART_BED_CH + 0.1 * millimeter; // el chaflán de cama no deja el borde del pasador por debajo de 0,8 mm
        const pinX = 3 * millimeter;
        const pinZ = definition.qrZ + qrHalf + MIN_FLOOR + PIN_D / 2;      // entre el QR y el borde superior de la barriga
        const inlayR = min(definition.qrR + INLAY_BORDER, inlayHalf - 0.5 * millimeter);
        const zCart0 = zSlotBottom - SLEEVE_T;                              // cara inferior del cartucho (apoya en el nervio)
        // Cara: visor redondeado con los dos ojos (el LED es la pupila) y la sonrisa entre ellos
        const eyeX = definition.ledS / 2;
        const visHalfX = eyeX + EYE_R + VISOR_BORDER;
        const visHalfZ = EYE_R + VISOR_BORDER;
        const visZ0 = definition.ledZ - visHalfZ;
        const visZ1 = definition.ledZ + visHalfZ;
        const visPinZ = definition.ledZ + VISOR_PIN_DZ;
        // Orejas con antena, en los costados de la cabeza
        const earY = D / 2;
        const antTopZ = H + definition.antH;                                 // centro de la bola de la antena
        // Peana (por debajo de Z = 0)
        const zBase = -definition.baseH;
        const xP0 = -definition.baseMS;
        const xP1 = W + definition.baseMS;
        const yP0 = -definition.backT - definition.baseMB;
        const yP1 = D + definition.baseMF;
        const toeX = definition.toeR + TOE_GAP / 2;                         // centro de cada pie desde el eje
        const chanHalf = definition.usbOW / 2 + CHAN_EXTRA;                 // canal del cable USB
        const chanEnd = usbY + definition.usbOT / 2 + CHAN_EXTRA;
        const nutAFc = definition.nutAF + 2 * definition.fitSlide;          // alojamiento hexagonal del lastre
        const nutRc = nutAFc / sqrt(3);
        const nutX = nutRc + 1 * millimeter;                                // tabique de 2 mm entre los dos alojamientos
        const nutY = chanEnd + BASE_WALL + nutAFc / 2;
        const basePinY = D - BASE_PIN_DY;

        // ====================== 2. COMPROBACIONES ======================
        if (pushOut > slotW - 0.4 * millimeter)
            throw regenError("El empujador ocuparía toda la ranura: reduce el margen en reposo o el recorrido tras el clic.");
        if (definition.hRest < leverRest + 0.2 * millimeter)
            throw regenError("En reposo la palanca no llega al empujador: el margen sobre el clic es mayor que el recorrido libre.");
        if (leverIn < definition.hPressed + definition.resid)
            throw regenError("Con el llavero dentro la palanca llegaría al final de carrera: reduce el recorrido tras el clic.");
        if (leverIn < definition.swH + 1 * millimeter)
            throw regenError("El tope del empujador choca con el cuerpo del microswitch.");
        if (tipFlat < PUSH_FLAT)
            throw regenError("Las dos rampas del empujador no caben en su alto: aumenta sus ángulos o reduce el margen en reposo o el recorrido tras el clic.");
        if (definition.pushW > 2 * slotHalf - 1 * millimeter)
            throw regenError("El empujador es más ancho que la ranura.");
        if (definition.pushW / 2 + COLLAR_X + 0.5 * millimeter > cx - wall - 2 * definition.bossR)
            throw regenError("El tope del empujador es demasiado ancho para la carcasa.");
        if (yb < definition.connSpace - 0.01 * millimeter)
            throw regenError("No queda espacio detrás del microswitch para el conector.");
        if (definition.pcbW / 2 - POST_INSET - POST_HALF < sleeveHalf + 0.5 * millimeter)
            throw regenError("La protoboard es demasiado estrecha: los postes de apriete chocan con la camisa de la ranura.");
        if (definition.plugClear < 2 * definition.bossR + definition.fitFirm + FENCE_T + 0.5 * millimeter)
            throw regenError("Hueco para la clavija USB demasiado bajo: los tetones inferiores chocan con la protoboard.");
        if (abs(swX - cx) + definition.swL / 2 + 0.5 * millimeter > sleeveHalf)
            throw regenError("El microswitch no cabe a lo ancho del cartucho: es demasiado largo o su rodillo está muy desplazado del centro (o aumenta el ancho del llavero).");
        if (definition.swScrews && definition.holeS / 2 + M2_CLEAR / 2 > definition.swL / 2)
            throw regenError("Los agujeros de montaje del microswitch quedan fuera de su cuerpo.");
        if (usbY - definition.usbOT / 2 < 0.5 * millimeter || usbY + definition.usbOT / 2 > D - wallF - 0.5 * millimeter)
            throw regenError("La abertura del USB no cabe en el fondo de la carcasa.");
        if (definition.cornerR > bIn &&
            sqrt(2) * (definition.cornerR - bIn) + max(definition.bossR, definition.cbD / 2 + 0.8 * millimeter) > definition.cornerR)
            throw regenError("Radio de esquinas demasiado grande: los tetones o los avellanados de la tapa atravesarían el canto redondeado.");
        if (wallF - PIN_IN_BODY < MIN_FLOOR - 0.01 * millimeter)
            throw regenError("Pared frontal demasiado fina para los agujeros de los pasadores (mínimo 1,8 mm).");
        if (wall - PIN_IN_BODY < MIN_FLOOR - 0.01 * millimeter)
            throw regenError("Paredes laterales demasiado finas para los pasadores de las orejas (mínimo 1,8 mm).");
        if (definition.qrDepth > INLAY_T - MIN_FLOOR + 0.01 * millimeter)
            throw regenError("El rebaje de la etiqueta QR es demasiado profundo para la barriga (máximo 0,8 mm).");
        if (inlayHalf > cx - definition.cornerR)
            throw regenError("La barriga (QR con su margen) es más ancha que la cara frontal.");
        if (inlayZ0 < definition.chamferF + MIN_FLOOR)
            throw regenError("La barriga queda demasiado baja: sube la etiqueta QR.");
        if (definition.cbDepth > definition.backT - 0.8 * millimeter)
            throw regenError("El avellanado es demasiado profundo para el grosor de la tapa trasera.");
        if (zSlotBottom - SLEEVE_T < zB0)
            throw regenError("La ranura es demasiado profunda para la altura de la carcasa.");
        // --- Ojos (LED) y cara ---
        if (eyeX - LED_FLANGE_R < sleeveHalf + 0.5 * millimeter && definition.ledZ + LED_FLANGE_R > zCart0 - RIB_SUP_T - 0.3 * millimeter)
            throw regenError("Los LED de los ojos chocan con el cartucho del mecanismo: sepáralos más o bájalos.");
        if (definition.ledZ + LED_FLANGE_R > H - wall - 1 * millimeter)
            throw regenError("Los ojos quedan demasiado altos: chocan con el techo.");
        for (var pz in [zB0 + POST_INSET, zB1 - POST_INSET])
        {
            if (abs(eyeX - (definition.pcbW / 2 - POST_INSET)) < POST_HALF + LED_FLANGE_R + 0.5 * millimeter &&
                abs(definition.ledZ - pz) < POST_HALF + LED_FLANGE_R + 0.5 * millimeter)
                throw regenError("Los LED de los ojos chocan con un poste de apriete de la protoboard.");
        }
        for (var bz in [bIn, H - bIn])
        {
            if (norm(vector(cx - eyeX - bIn, definition.ledZ - bz)) < definition.bossR + LED_FLANGE_R + 0.5 * millimeter)
                throw regenError("Los LED de los ojos chocan con un tetón de inserto.");
        }
        if (visHalfX > cx - definition.chamferF - 0.5 * millimeter)
            throw regenError("La cara (visor) es más ancha que la cara frontal: junta los ojos.");
        if (visZ1 > H - definition.cornerR)
            throw regenError("La cara (visor) queda demasiado alta: baja los ojos.");
        if (inlayZ1 + 0.5 * millimeter > visZ0)
            throw regenError("La barriga y la cara se solapan: baja la etiqueta QR o sube los ojos.");
        if (eyeX - EYE_R < SMILE_R + 1 * millimeter)
            throw regenError("Los ojos están tan juntos que no cabe la sonrisa entre ellos.");
        if (norm(vector(eyeX - VISOR_PIN_DX, VISOR_PIN_DZ)) < definition.ledD / 2 + PIN_D / 2 + MIN_FLOOR ||
            norm(vector(VISOR_PIN_DX, VISOR_PIN_DZ)) < SMILE_R + PIN_D / 2 + MIN_FLOOR ||
            VISOR_PIN_DZ + PIN_D / 2 + MIN_FLOOR > visHalfZ)
            throw regenError("Los pasadores de la cara no caben entre los ojos y la sonrisa.");
        // --- Orejas ---
        if (definition.earZ - definition.earR < definition.cornerR || definition.earZ + definition.earR > H - definition.cornerR)
            throw regenError("La oreja no cabe en la parte plana del costado: cambia su altura o su radio.");
        if (definition.earR > D / 2 - max(definition.chamferF, definition.chamferB) - 0.5 * millimeter)
            throw regenError("La oreja es más grande que el fondo del costado.");
        if (definition.earR < EAR_PIN_DY + PIN_D / 2 + MIN_FLOOR)
            throw regenError("Oreja demasiado pequeña para sus pasadores.");
        if (antTopZ - definition.antBall < definition.earZ + definition.earR)
            throw regenError("La bola de la antena se monta sobre el disco de la oreja: sube la antena.");
        // --- Peana ---
        if (nutY + nutAFc / 2 + BASE_WALL + PIN_D / 2 > basePinY)
            throw regenError("No caben los alojamientos del lastre delante del canal del cable: usa una tuerca más pequeña.");
        if (cx - nutX - nutRc < definition.cornerR + 0.5 * millimeter)
            throw regenError("Los alojamientos del lastre se salen de la base plana del cuerpo: usa una tuerca más pequeña.");
        if (BASE_PIN_DX + PIN_D / 2 > cx - definition.cornerR - 0.5 * millimeter)
            throw regenError("Los pasadores de la peana quedan fuera de la base plana del cuerpo.");
        if (definition.baseR > min(xP1 - xP0, yP1 - yP0) / 2 - 0.5 * millimeter)
            throw regenError("Radio de las esquinas de la peana demasiado grande.");
        if (!definition.swScrews)
            reportFeatureWarning(context, id, "Sin los tornillos M2 nada impide que la palanca empuje el microswitch hacia atrás: hay que pegarlo a las baldas.");

        // ====================== 3. CARCASA: ENVOLVENTE ======================
        caja(context, id + "outer", vector(ZERO, ZERO, ZERO), vector(W, D, H));
        opFillet(context, id + "outerFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "outer", EntityType.EDGE), Y_DIRECTION),
                    "radius" : definition.cornerR });
        const outerBody = qCreatedBy(id + "outer", EntityType.BODY);
        // Bisel 45° en la cara que apoya en la cama (frontal): evita pata de elefante y da el canto tipo "iPhone 5"
        opChamfer(context, id + "frontChamfer", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(outerBody, EntityType.EDGE), plane(vector(ZERO, D, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : definition.chamferF });
        opChamfer(context, id + "backChamfer", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(outerBody, EntityType.EDGE), plane(vector(ZERO, ZERO, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : definition.chamferB });

        // Vaciado (abierto por detrás)
        caja(context, id + "cavity", vector(wall, -1 * millimeter, wall), vector(W - wall, ysF, H - wall));
        opFillet(context, id + "cavityFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "cavity", EntityType.EDGE), Y_DIRECTION),
                    "radius" : max(definition.cornerR - wall, 0.5 * millimeter) });
        subtract(context, id + "cutCavity", outerBody, qCreatedBy(id + "cavity", EntityType.BODY));

        const bodyPoint = vector(wall / 2, D / 2, H / 2);                    // punto dentro de la pared lateral izquierda

        // ====================== 4. CARCASA: ELEMENTOS INTERIORES ======================
        const bossPts = [[bIn, bIn], [W - bIn, bIn], [bIn, H - bIn], [W - bIn, H - bIn]];
        for (var i = 0; i < 4; i += 1)
        {
            cylY(context, id + "inner" + ("boss" ~ toString(i)), bossPts[i][0], bossPts[i][1], ZERO, ysF + EPS, definition.bossR);
        }

        // Nervio de la pared frontal sobre el que apoya el cartucho (fija su altura: su techo queda enrasado)
        caja(context, id + "inner" + "cartRib",
            vector(cx - sleeveHalf, ysF - RIB_SUP_D, zCart0 - RIB_SUP_T),
            vector(cx + sleeveHalf, ysF + EPS, zCart0));

        // Postes que aprietan la protoboard contra los soportes de la tapa trasera
        const postX = [cx - (definition.pcbW / 2 - POST_INSET), cx + (definition.pcbW / 2 - POST_INSET)];
        const postZ = [zB0 + POST_INSET, zB1 - POST_INSET];
        var n = 0;
        for (var px in postX)
        {
            for (var pz in postZ)
            {
                caja(context, id + "inner" + ("post" ~ toString(n)),
                    vector(px - POST_HALF, definition.pcbY + definition.pcbT + definition.postGap, pz - POST_HALF),
                    vector(px + POST_HALF, ysF + EPS, pz + POST_HALF));
                // Nervio hasta la pared lateral: el poste mide más de 30 mm y sin él cimbrea al imprimir y al apretar
                const wallX = (px < cx) ? wall - EPS : W - wall + EPS;
                caja(context, id + "inner" + ("rib" ~ toString(n)),
                    vector(wallX, definition.pcbY + definition.pcbT + definition.postGap, pz - RIB_T / 2),
                    vector(px, ysF + EPS, pz + RIB_T / 2));
                n += 1;
            }
        }

        unite(context, id + "uniteBody", qUnion([bodyAt(id, bodyPoint), qCreatedBy(id + "inner", EntityType.BODY)]));

        // ====================== 5. CUERPO: VACIADOS ======================
        // Muesca del techo para el cartucho, abierta hacia atrás: en impresión (cara frontal abajo) queda abierta
        // hacia arriba, así que no hay puente. El cartucho la rellena con su franja de techo.
        caja(context, id + "cuts" + "roofNotch",
            vector(cx - sleeveHalf - definition.fitSlide, -1 * millimeter, H - wall - 1 * millimeter),
            vector(cx + sleeveHalf + definition.fitSlide, ysF, H + 1 * millimeter));
        // Agujeros para insertos térmicos
        for (var i = 0; i < 4; i += 1)
        {
            cylY(context, id + "cuts" + ("insert" ~ toString(i)), bossPts[i][0], bossPts[i][1], -1 * millimeter, definition.insertL, definition.insertD / 2);
        }
        // Paso de la clavija USB-C por la base
        caja(context, id + "cuts" + "usb",
            vector(usbX - definition.usbOW / 2, usbY - definition.usbOT / 2, -1 * millimeter),
            vector(usbX + definition.usbOW / 2, usbY + definition.usbOT / 2, wall + 0.5 * millimeter));
        opFillet(context, id + "cuts" + "usbFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "cuts" + "usb", EntityType.EDGE), Z_DIRECTION),
                    "radius" : 1.5 * millimeter });
        // Ranuras de ventilación en la base (largas en Y: en impresión el puente es de solo 1,6 mm)
        const ventOff = [definition.usbOW / 2 + 3 * millimeter, definition.usbOW / 2 + 6 * millimeter];
        var v = 0;
        for (var s in [-1, 1])
        {
            for (var o in ventOff)
            {
                const vx = usbX + s * o;
                if (vx - 0.8 * millimeter > wall + 2 * definition.bossR + 0.5 * millimeter &&
                    vx + 0.8 * millimeter < W - wall - 2 * definition.bossR - 0.5 * millimeter)
                {
                    caja(context, id + "cuts" + ("vent" ~ toString(v)),
                        vector(vx - 0.8 * millimeter, 3 * millimeter, -1 * millimeter),
                        vector(vx + 0.8 * millimeter, 10 * millimeter, wall + 0.5 * millimeter));
                }
                v += 1;
            }
        }
        for (var s in [-1, 1])
        {
            const sid = toString(s + 1);
            // Pasadores de la barriga y de la cara (en la cama al imprimir: puente de solo 1,9 mm)
            cylY(context, id + "cuts" + ("pin" ~ sid), cx + s * pinX, pinZ, D - PIN_IN_BODY, D + 1 * millimeter, PIN_D / 2);
            cylY(context, id + "cuts" + ("vpin" ~ sid), cx + s * VISOR_PIN_DX, visPinZ, D - PIN_IN_BODY, D + 1 * millimeter, PIN_D / 2);
            // Ojos: agujeros de los LED (atraviesan la pared frontal; la cara lleva los suyos)
            cylY(context, id + "cuts" + ("led" ~ sid), cx + s * eyeX, definition.ledZ,
                 ysF - 0.5 * millimeter, D + 1 * millimeter, definition.ledD / 2);
            // Pasadores de las orejas en los costados (horizontales al imprimir: puente de 1,9 mm)
            const x0 = (s < 0) ? ZERO : W;
            for (var t in [-1, 1])
            {
                fCylinder(context, id + "cuts" + ("epin" ~ sid ~ toString(t + 1)), {
                            "bottomCenter" : vector(x0 + s * 1 * millimeter, earY + t * EAR_PIN_DY, definition.earZ),
                            "topCenter" : vector(x0 - s * PIN_IN_BODY, earY + t * EAR_PIN_DY, definition.earZ),
                            "radius" : PIN_D / 2 });
            }
            // Pasadores de la peana en la base del cuerpo
            cylZ(context, id + "cuts" + ("bpin" ~ sid), cx + s * BASE_PIN_DX, basePinY, -1 * millimeter, PIN_IN_BODY, PIN_D / 2);
        }
        subtract(context, id + "doCuts", bodyAt(id, bodyPoint), qCreatedBy(id + "cuts", EntityType.BODY));

        // ====================== 5b. CARTUCHO DEL MECANISMO ======================
        // Se imprime tumbado sobre su costado -X (plano x = cx - sleeveHalf): todo crece hacia +X.
        //  · las caras de la ranura quedan verticales, con las capas a lo largo del deslizamiento del llavero
        //  · el techo del tubo (costado +X) es un puente de slotW (3,5 mm)
        //  · la guía hace de puente entre las dos baldas (hP + 2·holgura ≈ 6,2 mm)
        // Tubo de la ranura: paredes trasera, delantera, laterales y fondo
        caja(context, id + "cart" + "tube",
            vector(cx - sleeveHalf, ysB - SLEEVE_T, zCart0),
            vector(cx + sleeveHalf, ysF, H));
        // Franja central del techo: rellena la muesca del cuerpo hasta la tapa trasera, que la retiene
        caja(context, id + "cart" + "cap",
            vector(cx - sleeveHalf, CAP_GAP, H - wall),
            vector(cx + sleeveHalf, ysF, H));
        // Baldas del microswitch (del ancho del cartucho): el switch queda atrapado entre las dos
        caja(context, id + "cart" + "shelf",
            vector(cx - sleeveHalf, yb, shelfTop - SHELF_T),
            vector(cx + sleeveHalf, ysF, shelfTop));
        caja(context, id + "cart" + "shelf2",
            vector(cx - sleeveHalf, yb, shelf2Bot),
            vector(cx + sleeveHalf, ysF, shelf2Bot + SHELF_T));
        // Guía del empujador, pegada a la pared trasera del tubo
        caja(context, id + "cart" + "guide",
            vector(cx - definition.pushW / 2 - definition.fitGuide - GUIDE_T, yGuideB, zc - hP / 2 - definition.fitGuide - GUIDE_T),
            vector(cx + definition.pushW / 2 + definition.fitGuide + GUIDE_T, ysB - SLEEVE_T + EPS, zc + hP / 2 + definition.fitGuide + GUIDE_T));
        const cartPoint = vector(cx, min(yb, ysB - SLEEVE_T) / 2, H - wall / 2);   // dentro de la franja de techo
        unite(context, id + "cartUnite", qCreatedBy(id + "cart", EntityType.BODY));

        // Ranura del llavero (atraviesa el techo del cartucho)
        caja(context, id + "cartCuts" + "slot",
            vector(cx - slotHalf, ysB, zSlotBottom),
            vector(cx + slotHalf, ysS, H + 1 * millimeter));
        // Agujero de la guía del empujador: atraviesa la guía y la pared trasera del tubo hasta la ranura
        // (su alto coincide con el hueco entre baldas, así el empujador entra por detrás)
        caja(context, id + "cartCuts" + "guideHole",
            vector(cx - definition.pushW / 2 - definition.fitGuide, yGuideB - 1 * millimeter, zc - hP / 2 - definition.fitGuide),
            vector(cx + definition.pushW / 2 + definition.fitGuide, ysB + 0.05 * millimeter, zc + hP / 2 + definition.fitGuide));
        // Microswitch: paso M2 en la balda inferior + piloto pasante en la superior (M2 x 10 desde abajo;
        // la punta sale al hueco vacío de encima). Montar el switch en el cartucho antes de meterlo en el cuerpo.
        if (definition.swScrews)
        {
            for (var s in [-1, 1])
            {
                const hx = swX + s * definition.holeS / 2;
                const hy = yb + definition.holeY;
                cylZ(context, id + "cartCuts" + ("swHole" ~ toString(s + 1)), hx, hy,
                     shelfTop - SHELF_T - 1 * millimeter, shelfTop + 0.1 * millimeter, M2_CLEAR / 2);
                cylZ(context, id + "cartCuts" + ("swPilot" ~ toString(s + 1)), hx, hy,
                     shelf2Bot - 0.1 * millimeter, shelf2Bot + SHELF_T + 1 * millimeter, M2_PILOT / 2);
            }
        }
        subtract(context, id + "doCartCuts", bodyAt(id, cartPoint), qCreatedBy(id + "cartCuts", EntityType.BODY));

        // Chaflán de entrada de la ranura
        const cartEdges = qOwnedByBody(bodyAt(id, cartPoint), EntityType.EDGE);
        const midY = (ysB + ysS) / 2;
        // "try silent": si el chaflán falla solo queda el aviso propio, sin el error interno en la lista de avisos
        try silent
        {
            opChamfer(context, id + "slotChamfer", {
                        "entities" : qUnion([qContainsPoint(cartEdges, vector(cx, ysB, H)),
                                             qContainsPoint(cartEdges, vector(cx, ysS, H)),
                                             qContainsPoint(cartEdges, vector(cx - slotHalf, midY, H)),
                                             qContainsPoint(cartEdges, vector(cx + slotHalf, midY, H))]),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : SLOT_CHAMFER });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar la entrada de la ranura; el resto del modelo es válido.");
        }
        // Chaflán en las aristas que apoyan en la cama (costado -X)
        try silent
        {
            opChamfer(context, id + "cartBedChamfer", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, cartPoint), EntityType.EDGE),
                                                         plane(vector(cx - sleeveHalf, ZERO, ZERO), X_DIRECTION)),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : CART_BED_CH });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar la base del cartucho; se puede imprimir igual.");
        }

        // ====================== 5c. BARRIGA (placa pegada con el QR) ======================
        // Va pegada sobre la cara frontal. Se imprime con la cara trasera (y = D) sobre la cama y la cara vista arriba.
        caja(context, id + "inlay",
            vector(cx - inlayHalf, D, inlayZ0),
            vector(cx + inlayHalf, D + INLAY_T, inlayZ1));
        opFillet(context, id + "inlayFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "inlay", EntityType.EDGE), Y_DIRECTION),
                    "radius" : inlayR });
        const inlayPoint = vector(cx, D + 0.3 * millimeter, definition.qrZ);  // bajo el rebaje del QR
        try silent
        {
            opChamfer(context, id + "inlayBedChamfer", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, inlayPoint), EntityType.EDGE),
                                                         plane(vector(ZERO, D, ZERO), Y_DIRECTION)),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : CART_BED_CH });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar el canto de la barriga; se puede imprimir igual.");
        }
        // Rebaje de la etiqueta QR
        caja(context, id + "inlayCuts" + "qr",
            vector(cx - qrHalf, D + INLAY_T - definition.qrDepth, definition.qrZ - qrHalf),
            vector(cx + qrHalf, D + INLAY_T + 1 * millimeter, definition.qrZ + qrHalf));
        opFillet(context, id + "inlayCuts" + "qrFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "inlayCuts" + "qr", EntityType.EDGE), Y_DIRECTION),
                    "radius" : min(definition.qrR, qrHalf - 0.1 * millimeter) });
        for (var s in [-1, 1])
        {
            cylY(context, id + "inlayCuts" + ("pin" ~ toString(s + 1)), cx + s * pinX, pinZ,
                 D - 1 * millimeter, D + PIN_IN_INLAY, PIN_D / 2);
        }
        subtract(context, id + "doInlayCuts", bodyAt(id, inlayPoint), qCreatedBy(id + "inlayCuts", EntityType.BODY));

        // ====================== 5d. CARA (visor con los ojos y la sonrisa) ======================
        // Pegada sobre la cara frontal. Se imprime con la cara trasera (y = D) sobre la cama: los anillos de los ojos
        // crecen hacia arriba y la sonrisa es un rebaje abierto hacia arriba, así que no hay voladizos ni puentes.
        caja(context, id + "cara", vector(cx - visHalfX, D, visZ0), vector(cx + visHalfX, D + INLAY_T, visZ1));
        opFillet(context, id + "caraFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "cara", EntityType.EDGE), Y_DIRECTION),
                    "radius" : visHalfZ - 0.5 * millimeter });
        for (var s in [-1, 1])
        {
            cylY(context, id + "caraEye" + ("e" ~ toString(s + 1)), cx + s * eyeX, definition.ledZ,
                 D + INLAY_T - EPS, D + INLAY_T + EYE_RING_H, EYE_R);
        }
        unite(context, id + "caraUnite", qUnion([qCreatedBy(id + "cara", EntityType.BODY), qCreatedBy(id + "caraEye", EntityType.BODY)]));
        const caraPoint = vector(cx, D + 0.3 * millimeter, visZ1 - 1 * millimeter);
        try silent
        {
            opChamfer(context, id + "caraBedChamfer", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, caraPoint), EntityType.EDGE),
                                                         plane(vector(ZERO, D, ZERO), Y_DIRECTION)),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : CART_BED_CH });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar el canto de la cara; se puede imprimir igual.");
        }
        for (var s in [-1, 1])
        {
            const sid = toString(s + 1);
            cylY(context, id + "caraCuts" + ("led" ~ sid), cx + s * eyeX, definition.ledZ,
                 D - 1 * millimeter, D + INLAY_T + EYE_RING_H + 1 * millimeter, definition.ledD / 2);
            cylY(context, id + "caraCuts" + ("pin" ~ sid), cx + s * VISOR_PIN_DX, visPinZ,
                 D - 1 * millimeter, D + PIN_IN_INLAY, PIN_D / 2);
        }
        // Sonrisa: medio anillo (la mitad de abajo) grabado entre los ojos
        cylY(context, id + "smile", cx, definition.ledZ, D + INLAY_T - SMILE_DEPTH, D + INLAY_T + 1 * millimeter, SMILE_R);
        cylY(context, id + "smileIn", cx, definition.ledZ, D + INLAY_T - SMILE_DEPTH - 1 * millimeter, D + INLAY_T + 2 * millimeter, SMILE_R - SMILE_W);
        caja(context, id + "smileTop", vector(cx - SMILE_R - 1 * millimeter, D, definition.ledZ),
             vector(cx + SMILE_R + 1 * millimeter, D + INLAY_T + 2 * millimeter, definition.ledZ + SMILE_R + 1 * millimeter));
        subtract(context, id + "smileCut", qCreatedBy(id + "smile", EntityType.BODY),
                 qUnion([qCreatedBy(id + "smileIn", EntityType.BODY), qCreatedBy(id + "smileTop", EntityType.BODY)]));
        subtract(context, id + "doCaraCuts", bodyAt(id, caraPoint),
                 qUnion([qCreatedBy(id + "caraCuts", EntityType.BODY), qCreatedBy(id + "smile", EntityType.BODY)]));

        // ====================== 5e. OREJAS CON ANTENA (dos piezas iguales y planas) ======================
        // Se imprimen tumbadas sobre su cara interior (la que se pega al costado): disco, palo y bola son un prisma
        // y el botón crece hacia arriba, sin voladizos. Centradas con 2 pasadores de filamento.
        var earPoints = [];
        for (var s in [-1, 1])
        {
            const eid = id + ("ear" ~ toString(s + 1));
            const x0 = (s < 0) ? ZERO : W;
            fCylinder(context, eid + "disc", { "bottomCenter" : vector(x0, earY, definition.earZ),
                        "topCenter" : vector(x0 + s * EAR_T, earY, definition.earZ), "radius" : definition.earR });
            caja(context, eid + "stick", vector(x0, earY - ANT_W / 2, definition.earZ), vector(x0 + s * EAR_T, earY + ANT_W / 2, antTopZ));
            fCylinder(context, eid + "ball", { "bottomCenter" : vector(x0, earY, antTopZ),
                        "topCenter" : vector(x0 + s * EAR_T, earY, antTopZ), "radius" : definition.antBall });
            fCylinder(context, eid + "knob", { "bottomCenter" : vector(x0 + s * (EAR_T - EPS), earY, definition.earZ),
                        "topCenter" : vector(x0 + s * (EAR_T + EAR_KNOB_H), earY, definition.earZ), "radius" : definition.earR - EAR_KNOB_IN });
            unite(context, eid + "unite", qCreatedBy(eid, EntityType.BODY));
            const ep = vector(x0 + s * EAR_T / 2, earY, definition.earZ);
            try silent
            {
                opChamfer(context, eid + "bedChamfer", {
                            "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, ep), EntityType.EDGE), plane(vector(x0, ZERO, ZERO), X_DIRECTION)),
                            "chamferType" : ChamferType.EQUAL_OFFSETS,
                            "width" : CART_BED_CH });
            }
            catch
            {
                reportFeatureWarning(context, id, "No se pudo achaflanar el canto de una oreja; se puede imprimir igual.");
            }
            for (var t in [-1, 1])
            {
                fCylinder(context, eid + "cuts" + ("pin" ~ toString(t + 1)), {
                            "bottomCenter" : vector(x0 - s * 1 * millimeter, earY + t * EAR_PIN_DY, definition.earZ),
                            "topCenter" : vector(x0 + s * PIN_IN_INLAY, earY + t * EAR_PIN_DY, definition.earZ),
                            "radius" : PIN_D / 2 });
            }
            subtract(context, eid + "doCuts", bodyAt(id, ep), qCreatedBy(eid + "cuts", EntityType.BODY));
            earPoints = append(earPoints, ep);
        }

        // ====================== 5f. PEANA ======================
        // Se imprime con la cara inferior sobre la cama. Todos sus huecos (canal del cable USB, alojamientos del lastre y
        // pasadores) se abren hacia arriba: no hay puentes. El cuerpo se pega encima y deja las tuercas encerradas.
        caja(context, id + "base", vector(xP0, yP0, zBase), vector(xP1, yP1, ZERO));
        opFillet(context, id + "baseFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "base", EntityType.EDGE), Z_DIRECTION),
                    "radius" : definition.baseR });
        if (definition.toeR > 0.01 * millimeter)
        {
            for (var s in [-1, 1])
            {
                cylZ(context, id + "toes" + ("t" ~ toString(s + 1)), cx + s * toeX, yP1, zBase, ZERO, definition.toeR);
            }
            unite(context, id + "baseUnite", qUnion([qCreatedBy(id + "base", EntityType.BODY), qCreatedBy(id + "toes", EntityType.BODY)]));
        }
        const basePoint = vector(cx, nutY + nutAFc / 2 + 0.6 * millimeter, zBase / 2);
        try silent
        {
            opFillet(context, id + "baseTopRound", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, basePoint), EntityType.EDGE), plane(vector(ZERO, ZERO, ZERO), Z_DIRECTION)),
                        "radius" : BASE_TOP_R });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo redondear el canto superior de la peana.");
        }
        try silent
        {
            opChamfer(context, id + "baseBedChamfer", {
                        "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, basePoint), EntityType.EDGE), plane(vector(ZERO, ZERO, zBase), Z_DIRECTION)),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : BASE_BED_CH });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar la base de la peana; se puede imprimir igual.");
        }
        // Canal del cable USB: baja del paso USB del cuerpo y sale por detrás
        caja(context, id + "baseCuts" + "chan", vector(usbX - chanHalf, yP0 - 1 * millimeter, zBase + BASE_FLOOR),
             vector(usbX + chanHalf, chanEnd, 1 * millimeter));
        for (var s in [-1, 1])
        {
            const sid = toString(s + 1);
            // Alojamiento hexagonal para tuercas de lastre (2 tuercas M8 apiladas en cada uno)
            const nutSk = newSketchOnPlane(context, id + "baseCuts" + ("nutSk" ~ sid), {
                        "sketchPlane" : plane(vector(ZERO, ZERO, zBase + BASE_FLOOR), Z_DIRECTION, X_DIRECTION) });
            skRegularPolygon(nutSk, "hex", { "center" : vector(cx + s * nutX, nutY),
                        "firstVertex" : vector(cx + s * nutX + nutRc, nutY), "sides" : 6 });
            skSolve(nutSk);
            opExtrude(context, id + "baseCuts" + ("nut" ~ sid), {
                        "entities" : qSketchRegion(id + "baseCuts" + ("nutSk" ~ sid)),
                        "direction" : Z_DIRECTION,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : definition.baseH });
            opDeleteBodies(context, id + "baseCuts" + ("nutSkDel" ~ sid), { "entities" : qCreatedBy(id + "baseCuts" + ("nutSk" ~ sid), EntityType.BODY) });
            cylZ(context, id + "baseCuts" + ("pin" ~ sid), cx + s * BASE_PIN_DX, basePinY, -BASE_PIN_IN, 1 * millimeter, PIN_D / 2);
        }
        subtract(context, id + "doBaseCuts", bodyAt(id, basePoint), qCreatedBy(id + "baseCuts", EntityType.BODY));

        // ====================== 6. TAPA TRASERA ======================
        const yBack = -definition.backT;
        caja(context, id + "back", vector(ZERO, yBack, ZERO), vector(W, ZERO, H));
        opFillet(context, id + "backFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "back", EntityType.EDGE), Y_DIRECTION),
                    "radius" : definition.cornerR });
        opChamfer(context, id + "backWallChamfer", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(qCreatedBy(id + "back", EntityType.BODY), EntityType.EDGE),
                                                     plane(vector(ZERO, yBack, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : definition.chamferB });

        // Soportes + escuadras en las 4 esquinas de la protoboard
        var f = 0;
        for (var sx in [-1, 1])
        {
            for (var zs in [[zB0, -1], [zB1, 1]])
            {
                const zEdge = zs[0];
                const sz = zs[1];
                caja(context, id + "fences" + ("f" ~ toString(f)),
                    vector(cx + sx * (definition.pcbW / 2 + definition.fitFirm + FENCE_T), -EPS, zEdge + sz * (definition.fitFirm + FENCE_T)),
                    vector(cx + sx * (definition.pcbW / 2 - FENCE_IN), definition.pcbY + definition.pcbT, zEdge - sz * FENCE_IN));
                f += 1;
            }
        }
        const backPoint = vector(wall, yBack / 2, H / 2);
        unite(context, id + "uniteBack", qUnion([qCreatedBy(id + "back", EntityType.BODY), qCreatedBy(id + "fences", EntityType.BODY)]));

        // Alojamiento de la protoboard (deja soporte debajo y escuadra alrededor)
        caja(context, id + "backCuts" + "pocket",
            vector(cx - definition.pcbW / 2 - definition.fitFirm, definition.pcbY, zB0 - definition.fitFirm),
            vector(cx + definition.pcbW / 2 + definition.fitFirm, definition.pcbY + definition.pcbT + 5 * millimeter, zB1 + definition.fitFirm));
        // Tornillos M3 con avellanado plano desde la cara de pared
        for (var i = 0; i < 4; i += 1)
        {
            // Con avellanado, el paso del tornillo empieza una capa por encima de su fondo: el fondo se imprime como un
            // puente circular apoyado en todo su borde (en vez de un anillo en voladizo) y se perfora al montar.
            const screwY0 = (definition.cbDepth > 0.01 * millimeter) ? yBack + definition.cbDepth + SAC_LAYER : yBack - 1 * millimeter;
            cylY(context, id + "backCuts" + ("screw" ~ toString(i)), bossPts[i][0], bossPts[i][1], screwY0, 1 * millimeter, definition.screwD / 2);
            if (definition.cbDepth > 0.01 * millimeter)
            {
                cylY(context, id + "backCuts" + ("cb" ~ toString(i)), bossPts[i][0], bossPts[i][1], yBack - 1 * millimeter, yBack + definition.cbDepth, definition.cbD / 2);
            }
        }
        subtract(context, id + "doBackCuts", bodyAt(id, backPoint), qCreatedBy(id + "backCuts", EntityType.BODY));

        // ====================== 7. EMPUJADOR (pieza suelta, dibujada en su posición de reposo) ======================
        // Perfil lateral (Y, Z) extruido en X: vástago con punta en cuña doble (rampa de entrada arriba,
        // rampa de salida abajo y una parte recta corta en la punta, que es donde apoya el llavero)
        const zb = zc - hP / 2;
        const zt = zc + hP / 2;
        const pushSk = newSketchOnPlane(context, id + "pushSk", {
                    "sketchPlane" : plane(vector(cx - definition.pushW / 2, ZERO, ZERO), X_DIRECTION, Y_DIRECTION) });
        skPolyline(pushSk, "perfil", { "points" : [
                        vector(yCollarB, zb), vector(ysB, zb), vector(yTip, zb + rampZout),
                        vector(yTip, zt - rampZin), vector(ysB, zt), vector(yCollarB, zt), vector(yCollarB, zb)] });
        skSolve(pushSk);
        opExtrude(context, id + "pushBody", {
                    "entities" : qSketchRegion(id + "pushSk"),
                    "direction" : X_DIRECTION,
                    "endBound" : BoundingType.BLIND,
                    "endDepth" : definition.pushW });
        opDeleteBodies(context, id + "pushSkDel", { "entities" : qCreatedBy(id + "pushSk", EntityType.BODY) });
        // Tope: más ancho que el agujero de la guía; el rodillo apoya en su cara trasera
        caja(context, id + "pushCollar",
            vector(cx - definition.pushW / 2 - COLLAR_X, yCollarB, zb),
            vector(cx + definition.pushW / 2 + COLLAR_X, yGuideB, zt));
        const pushPoint = vector(cx, ysB - SLEEVE_T / 2, zc);               // punto dentro del vástago, en la guía
        unite(context, id + "pushUnite", qUnion([qCreatedBy(id + "pushBody", EntityType.BODY), qCreatedBy(id + "pushCollar", EntityType.BODY)]));
        const pushEdges = qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE);
        // Cuña redondeada: redondeo donde las rampas nacen del vástago y, más pequeño, en la punta
        // (el de la punta se limita para que siga quedando parte recta donde apoya el llavero)
        // Cada redondeo consume r·tan(ángulo de rampa / 2) de la parte recta; se dejan al menos 0,3 mm rectos
        const tipRound = min(PUSH_ROUND, (tipFlat - 0.3 * millimeter) / (tan(definition.rampIn / 2) + tan(definition.rampOut / 2)));
        try silent
        {
            opFillet(context, id + "pushRound", {
                        "entities" : qUnion([qContainsPoint(pushEdges, vector(cx, ysB, zt)),
                                             qContainsPoint(pushEdges, vector(cx, ysB, zb))]),
                        "radius" : PUSH_ROUND });
            opFillet(context, id + "pushTipRound", {
                        "entities" : qUnion([qContainsPoint(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE), vector(cx, yTip, zt - rampZin)),
                                             qContainsPoint(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE), vector(cx, yTip, zb + rampZout))]),
                        "radius" : tipRound });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo redondear la cuña del empujador; queda en arista viva.");
        }
        // Se imprime de pie sobre la cara trasera del tope: chaflán en las aristas que apoyan en la cama
        opChamfer(context, id + "pushBedChamfer", {
                    "entities" : qCoincidesWithPlane(qOwnedByBody(bodyAt(id, pushPoint), EntityType.EDGE),
                                                     plane(vector(ZERO, yCollarB, ZERO), Y_DIRECTION)),
                    "chamferType" : ChamferType.EQUAL_OFFSETS,
                    "width" : PUSH_BED_CH });

        // ====================== 8. PIEZAS DE PRUEBA (opcional) ======================
        // Recortes de las piezas reales, en su sitio, para imprimir poco material y medir los ajustes:
        //  0 "Prueba encaje cartucho":     muesca del techo, pared frontal y nervio de apoyo del cuerpo
        //    (el mecanismo se prueba con el Cartucho y el Empujador reales: son piezas pequeñas)
        //  1 "Prueba esquina inserto M3":  esquina inferior izquierda de la carcasa, últimos mm del tetón
        //  2 "Prueba paso USB":            trozo de la base con la abertura de la clavija
        //  3 "Prueba tapa y protoboard":   marco de la tapa con las 4 escuadras, soportes y avellanados inferiores
        var massInfo = "";
        if (definition.testMode)
        {
            const xm = sleeveHalf + definition.fitSlide + 2.5 * millimeter;
            const zm0 = zCart0 - RIB_SUP_T - 1 * millimeter;
            // Corte de la esquina: si cae sobre una ranura de ventilación (o a menos de 0,8 mm), la incluye entera
            var xc1 = bIn + definition.bossR + 4 * millimeter;
            for (var o in [ventOff[1], ventOff[0]])
            {
                const vx = usbX - o;
                if (vx - 0.8 * millimeter > wall + 2 * definition.bossR + 0.5 * millimeter &&
                    xc1 > vx - 1.6 * millimeter && xc1 < vx + 1.6 * millimeter)
                    xc1 = vx + 1.6 * millimeter;
            }
            // En altura no llega al poste de apriete inferior ni a su nervio
            const zc1 = max(bIn + definition.bossR + 1 * millimeter, min(xc1, zB0 + POST_INSET - POST_HALF - 1 * millimeter));
            const yc1 = definition.insertL + 5 * millimeter;                 // pasa de las ranuras de ventilación (y = 10)
            const xu = definition.usbOW / 2 + 2 * millimeter;               // no llega a las ranuras de ventilación
            const yu = definition.usbOT / 2 + 3 * millimeter;
            // Ventana del marco: deja las escuadras
            const xw = definition.pcbW / 2 - FENCE_IN;
            const zw0 = zB0 + FENCE_IN;
            const zw1 = zB1 - FENCE_IN;
            const zf1 = max(zB1 + definition.fitFirm + FENCE_T + 2 * millimeter, zw1 + 3 * millimeter);
            const specs = [
                [bodyPoint, vector(cx - xm, yb - 1 * millimeter, zm0), vector(cx + xm, D + 1 * millimeter, H + 1 * millimeter)],
                [bodyPoint, vector(-1 * millimeter, -1 * millimeter, -1 * millimeter), vector(xc1, yc1, zc1)],
                [bodyPoint, vector(usbX - xu, usbY - yu, -1 * millimeter), vector(usbX + xu, usbY + yu, wall)],
                [backPoint, vector(-1 * millimeter, yBack - 1 * millimeter, -1 * millimeter), vector(W + 1 * millimeter, 10 * millimeter, zf1)]];
            for (var t = 0; t < size(specs); t += 1)
            {
                const tid = toString(t);
                opPattern(context, id + "test" + ("cp" ~ tid), {
                            "entities" : qSubtraction(bodyAt(id, specs[t][0]), qCreatedBy(id + "test", EntityType.BODY)),
                            "transforms" : [identityTransform()],
                            "instanceNames" : ["c" ~ tid] });
                caja(context, id + "test" + ("box" ~ tid), specs[t][1], specs[t][2]);
                opBoolean(context, id + "test" + ("cut" ~ tid), {
                            "tools" : qUnion([qCreatedBy(id + "test" + ("cp" ~ tid), EntityType.BODY), qCreatedBy(id + "test" + ("box" ~ tid), EntityType.BODY)]),
                            "operationType" : BooleanOperationType.INTERSECTION });
            }
            // Cada pieza de prueba se identifica por un punto interior, entre los sólidos creados en este bloque
            // (no depende de a qué operación atribuya Onshape el resultado de la intersección)
            const couponPts = [vector(cx, ysF + wallF / 2, zCart0 + 2 * millimeter),               // en la pared frontal
                               vector(bIn, definition.insertL + 2.5 * millimeter, bIn),   // en el tetón, pasado el inserto
                               vector(usbX + definition.usbOW / 2 + 1 * millimeter, usbY, wall / 2),
                               vector(cx, yBack / 2, zw0 / 2)];                          // franja inferior del marco, en el centro
            const coupon = function(t) returns Query
            {
                return qIntersection([bodyAt(id, couponPts[t]), qCreatedBy(id + "test", EntityType.BODY)]);
            };
            // Ventana central del marco de la tapa (deja las escuadras y una franja alrededor)
            caja(context, id + "test" + "window", vector(cx - xw, yBack - 1 * millimeter, zw0), vector(cx + xw, 10 * millimeter, zw1));
            subtract(context, id + "test" + "windowCut", coupon(3), qCreatedBy(id + "test" + "window", EntityType.BODY));
            // Se quitan las piezas completas
            opDeleteBodies(context, id + "test" + "delOrig", {
                        "entities" : qSubtraction(qUnion([bodyAt(id, bodyPoint), bodyAt(id, backPoint), bodyAt(id, inlayPoint),
                                                          bodyAt(id, caraPoint), bodyAt(id, earPoints[0]), bodyAt(id, earPoints[1]), bodyAt(id, basePoint)]),
                                                  qCreatedBy(id + "test", EntityType.BODY)) });

            // Chaflán en las aristas de corte que apoyan en la cama (pata de elefante)
            const mechEdges = qOwnedByBody(coupon(0), EntityType.EDGE);
            const frameEdges = qOwnedByBody(coupon(3), EntityType.EDGE);
            const bedEdges = [
                qUnion([qContainsPoint(mechEdges, vector(cx - xm, D, (zm0 + H) / 2)),
                        qContainsPoint(mechEdges, vector(cx + xm, D, (zm0 + H) / 2)),
                        qContainsPoint(mechEdges, vector(cx, D, zm0))]),
                // esquina: solo el contorno exterior (el interior del tetón tiene aristas concurrentes que no admiten chaflán)
                qUnion([qContainsPoint(qOwnedByBody(coupon(1), EntityType.EDGE), vector(xc1, yc1, wall / 2)),
                        qContainsPoint(qOwnedByBody(coupon(1), EntityType.EDGE), vector(wall / 2, yc1, zc1)),
                        qContainsPoint(qOwnedByBody(coupon(1), EntityType.EDGE), vector((definition.cornerR + xc1) / 2, yc1, ZERO)),
                        qContainsPoint(qOwnedByBody(coupon(1), EntityType.EDGE), vector(ZERO, yc1, (definition.cornerR + zc1) / 2)),
                        qContainsPoint(qOwnedByBody(coupon(1), EntityType.EDGE),
                                       vector(definition.cornerR * (1 - cos(45 * degree)), yc1, definition.cornerR * (1 - cos(45 * degree))))]),
                qCoincidesWithPlane(qOwnedByBody(coupon(2), EntityType.EDGE), plane(vector(ZERO, ZERO, ZERO), Z_DIRECTION)),
                qUnion([qContainsPoint(frameEdges, vector(cx, yBack, zf1)),
                        qContainsPoint(frameEdges, vector(cx - xw, yBack, (zw0 + zw1) / 2)),
                        qContainsPoint(frameEdges, vector(cx + xw, yBack, (zw0 + zw1) / 2)),
                        qContainsPoint(frameEdges, vector(cx + xw / 2, yBack, zw0)),
                        qContainsPoint(frameEdges, vector(cx + xw / 2, yBack, zw1))])];
            const names = ["Prueba encaje cartucho", "Prueba esquina inserto M3", "Prueba paso USB", "Prueba tapa y protoboard"];
            for (var t = 0; t < size(bedEdges); t += 1)
            {
                try silent
                {
                    opChamfer(context, id + "test" + ("ch" ~ toString(t)), {
                                "entities" : bedEdges[t],
                                "chamferType" : ChamferType.EQUAL_OFFSETS,
                                "width" : COUPON_CH });
                }
                catch
                {
                    reportFeatureWarning(context, id, "No se pudo achaflanar la base de \"" ~ names[t] ~ "\"; se puede imprimir igual.");
                }
            }
            for (var t = 0; t < size(names); t += 1)
            {
                setProperty(context, { "entities" : coupon(t), "propertyType" : PropertyType.NAME, "value" : names[t] });
            }
            const grams = evVolume(context, { "entities" : qUnion([qCreatedBy(id + "test", EntityType.BODY), bodyAt(id, pushPoint), bodyAt(id, cartPoint)]) }) /
                          (centimeter ^ 3) * PETG_DENSITY;
            massInfo = "PIEZAS DE PRUEBA (" ~ toString(roundToPrecision(grams, 0)) ~ " g de PETG como mucho, al 100 % de relleno) · ";
        }

        // ====================== 9. NOMBRES, EXPLOSIÓN E INFORME ======================
        if (!definition.testMode)
        {
            setProperty(context, { "entities" : bodyAt(id, bodyPoint), "propertyType" : PropertyType.NAME, "value" : "Cuerpo" });
            setProperty(context, { "entities" : bodyAt(id, backPoint), "propertyType" : PropertyType.NAME, "value" : "Tapa trasera" });
            setProperty(context, { "entities" : bodyAt(id, inlayPoint), "propertyType" : PropertyType.NAME, "value" : "Barriga" });
            setProperty(context, { "entities" : bodyAt(id, caraPoint), "propertyType" : PropertyType.NAME, "value" : "Cara" });
            setProperty(context, { "entities" : bodyAt(id, earPoints[0]), "propertyType" : PropertyType.NAME, "value" : "Oreja izquierda" });
            setProperty(context, { "entities" : bodyAt(id, earPoints[1]), "propertyType" : PropertyType.NAME, "value" : "Oreja derecha" });
            setProperty(context, { "entities" : bodyAt(id, basePoint), "propertyType" : PropertyType.NAME, "value" : "Peana" });
        }
        setProperty(context, { "entities" : bodyAt(id, cartPoint), "propertyType" : PropertyType.NAME, "value" : "Cartucho mecanismo" });
        setProperty(context, { "entities" : bodyAt(id, pushPoint), "propertyType" : PropertyType.NAME, "value" : "Empujador" });
        if (definition.explode > 0.01 * millimeter)
        {
            opTransform(context, id + "explode", {
                        "bodies" : bodyAt(id, backPoint),
                        "transform" : transform(vector(ZERO, -definition.explode, ZERO)) });
            if (!definition.testMode)
            {
                opTransform(context, id + "explodeInlay", {
                            "bodies" : qUnion([bodyAt(id, inlayPoint), bodyAt(id, caraPoint)]),
                            "transform" : transform(vector(ZERO, definition.explode, ZERO)) });
                opTransform(context, id + "explodeEarL", {
                            "bodies" : bodyAt(id, earPoints[0]),
                            "transform" : transform(vector(-definition.explode, ZERO, ZERO)) });
                opTransform(context, id + "explodeEarR", {
                            "bodies" : bodyAt(id, earPoints[1]),
                            "transform" : transform(vector(definition.explode, ZERO, ZERO)) });
                opTransform(context, id + "explodeBase", {
                            "bodies" : bodyAt(id, basePoint),
                            "transform" : transform(vector(ZERO, ZERO, -definition.explode)) });
            }
        }
        const toeFront = (definition.toeR > 0.01 * millimeter) ? yP1 + definition.toeR : yP1;
        reportFeatureInfo(context, id, massInfo ~ "Keko: cuerpo " ~ toString(roundToPrecision(W / millimeter, 1)) ~ " x " ~
                    toString(roundToPrecision((D + definition.backT + INLAY_T + EYE_RING_H) / millimeter, 1)) ~ " x " ~ toString(roundToPrecision(H / millimeter, 1)) ~
                    " mm; peana " ~ toString(roundToPrecision((xP1 - xP0) / millimeter, 1)) ~ " x " ~ toString(roundToPrecision((toeFront - yP0) / millimeter, 1)) ~
                    " mm; alto total " ~ toString(roundToPrecision((definition.baseH + antTopZ + definition.antBall) / millimeter, 1)) ~
                    " mm · empujador asoma " ~ toString(roundToPrecision(pushOut / millimeter, 2)) ~ " mm (rampas de " ~
                    toString(roundToPrecision(definition.rampIn / degree, 0)) ~ " y " ~ toString(roundToPrecision(definition.rampOut / degree, 0)) ~
                    " grados, punta recta de " ~ toString(roundToPrecision(tipFlat / millimeter, 2)) ~ " mm) · palanca " ~
                    toString(roundToPrecision(leverRest / millimeter, 2)) ~
                    " mm en reposo y " ~ toString(roundToPrecision(leverIn / millimeter, 2)) ~ " mm con llavero (clic a " ~
                    toString(roundToPrecision(definition.hOP / millimeter, 2)) ~ " mm)");
    }, {
        // Valores por defecto (iguales a los de los límites B_*): los usan las pruebas automáticas
        // y las instancias antiguas de la feature si se añaden parámetros nuevos
        "wall" : 2.0 * millimeter, "wallF" : 2.0 * millimeter, "cornerR" : 6.0 * millimeter,
        "chamferF" : 1.0 * millimeter, "chamferB" : 0.5 * millimeter, "plugClear" : 10.0 * millimeter,
        "pcbW" : 40.0 * millimeter, "pcbH" : 50.0 * millimeter, "pcbT" : 1.6 * millimeter, "pcbY" : 4.0 * millimeter,
        "postGap" : 0.0 * millimeter, "headerH" : 8.5 * millimeter, "espFront" : 4.8 * millimeter, "usbC" : 3.15 * millimeter,
        "espOffX" : 0.0 * millimeter, "usbOW" : 13.5 * millimeter, "usbOT" : 8.0 * millimeter,
        "tagW" : 22.0 * millimeter, "tagT" : 3.0 * millimeter, "slotDepth" : 28.0 * millimeter,
        "fitSlide" : 0.25 * millimeter, "fitFirm" : 0.15 * millimeter,
        "swL" : 20.0 * millimeter, "swT" : 6.0 * millimeter, "swH" : 8.4 * millimeter, "hPressed" : 12.2 * millimeter,
        "hRest" : 20.0 * millimeter, "hOP" : 17.0 * millimeter, "resid" : 0.3 * millimeter, "fitSw" : 0.1 * millimeter,
        "connSpace" : 11.0 * millimeter, "rollerX" : 0.0 * millimeter,
        "swScrews" : true, "holeS" : 9.5 * millimeter, "holeY" : 2.5 * millimeter,
        "pushZ" : 9.0 * millimeter, "restMargin" : 1.5 * millimeter, "overTravel" : 0.8 * millimeter,
        "pushW" : 8.0 * millimeter, "guideL" : 4.0 * millimeter, "fitGuide" : 0.25 * millimeter,
        "rampIn" : 45 * degree, "rampOut" : 52 * degree,
        "backT" : 3.0 * millimeter, "insertD" : 4.0 * millimeter, "insertL" : 6.0 * millimeter, "bossR" : 3.5 * millimeter,
        "screwD" : 3.4 * millimeter, "cbD" : 6.5 * millimeter, "cbDepth" : 1.7 * millimeter,
        "qrSize" : 30.0 * millimeter, "qrFit" : 0.2 * millimeter, "qrDepth" : 0.4 * millimeter, "qrR" : 2.0 * millimeter,
        "qrZ" : 24.0 * millimeter,
        "ledD" : 3.1 * millimeter, "ledS" : 32.0 * millimeter, "ledZ" : 72.0 * millimeter,
        "earR" : 7.0 * millimeter, "earZ" : 72.0 * millimeter, "antH" : 9.0 * millimeter, "antBall" : 3.5 * millimeter,
        "baseH" : 15.0 * millimeter, "baseMS" : 6.0 * millimeter, "baseMF" : 6.0 * millimeter, "baseMB" : 6.0 * millimeter,
        "baseR" : 10.0 * millimeter, "toeR" : 10.0 * millimeter, "nutAF" : 13.0 * millimeter,
        "explode" : 0.0 * millimeter, "testMode" : false });

// ---------------- Funciones auxiliares ----------------
// Prisma recto entre dos esquinas opuestas ("box" es palabra reservada en FeatureScript)
function caja(context is Context, id is Id, p0 is Vector, p1 is Vector)
{
    fCuboid(context, id, {
                "corner1" : vector(min(p0[0], p1[0]), min(p0[1], p1[1]), min(p0[2], p1[2])),
                "corner2" : vector(max(p0[0], p1[0]), max(p0[1], p1[1]), max(p0[2], p1[2])) });
}

// Cilindro con eje paralelo a Y (centro en x, z)
function cylY(context is Context, id is Id, x, z, y0, y1, r)
{
    fCylinder(context, id, { "bottomCenter" : vector(x, y0, z), "topCenter" : vector(x, y1, z), "radius" : r });
}

// Cilindro con eje paralelo a Z (centro en x, y)
function cylZ(context is Context, id is Id, x, y, z0, z1, r)
{
    fCylinder(context, id, { "bottomCenter" : vector(x, y, z0), "topCenter" : vector(x, y, z1), "radius" : r });
}

// Sólido que contiene un punto (sigue a la pieza aunque las booleanas cambien su identidad)
// Solo entre los sólidos que crea esta feature: no toca otras piezas del Part Studio que contengan el punto
function bodyAt(id is Id, p is Vector) returns Query
{
    return qContainsPoint(qBodyType(qCreatedBy(id, EntityType.BODY), BodyType.SOLID), p);
}

function subtract(context is Context, id is Id, target is Query, tools is Query)
{
    opBoolean(context, id, { "tools" : tools, "targets" : target, "operationType" : BooleanOperationType.SUBTRACTION });
}

function unite(context is Context, id is Id, bodies is Query)
{
    opBoolean(context, id, { "tools" : bodies, "operationType" : BooleanOperationType.UNION });
}
