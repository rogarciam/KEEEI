FeatureScript 3112;
import(path : "onshape/std/common.fs", version : "3112.0");

/*
 * Carcasa IoT detector de llaves — ESP32 NodeMCU-32S (USB-C) + microswitch KW12-3 con rodillo
 * Concepto C: ranura vertical con LEVA LATERAL. El llavero entra por arriba y, cerca del fondo de la
 * ranura, empuja de lado un EMPUJADOR con cuña redondeada que corre en una guía horizontal y aprieta el
 * rodillo del microswitch. El empujador tiene un tope: en reposo deja la palanca justo por encima del
 * clic y con el llavero dentro la lleva un poco por debajo, sin llegar al final de carrera.
 *
 * Ejes:  X = ancho · Y = profundidad (Y=0 tapa trasera / pared, Y=D cara frontal) · Z = altura (Z=0 base)
 * Piezas: "Carcasa frontal" (imprimir con la cara frontal sobre la cama)
 *         "Tapa trasera"    (imprimir con la cara de pared sobre la cama)
 *         "Empujador"       (imprimir de pie, apoyado en la cara del tope; se mete por detrás antes que el switch)
 * Unión:  4 tornillos M3 desde la tapa a insertos térmicos M3 en la carcasa.
 * Montaje en pared: 2 ojales (keyholes) en la tapa trasera.
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
const SLEEVE_T     = 1.2 * millimeter;  // pared de la camisa de la ranura
const SHELF_T      = 1.6 * millimeter;  // balda del microswitch
const SLOT_CHAMFER = 0.8 * millimeter;  // chaflán de entrada de la ranura
const KEY_LEDGE    = 1.2 * millimeter;  // reborde que retiene la cabeza del tornillo de pared
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
const B_KEY_Z1    = { (millimeter) : [5.0, 22.0, 200.0] } as LengthBoundSpec;
const B_KEY_Z2    = { (millimeter) : [5.0, 52.0, 200.0] } as LengthBoundSpec;
const B_KEY_HEAD  = { (millimeter) : [5.0, 8.5, 12.0] } as LengthBoundSpec;
const B_KEY_SHANK = { (millimeter) : [2.5, 4.2, 6.0] } as LengthBoundSpec;
const B_KEY_SLOT  = { (millimeter) : [4.0, 8.0, 15.0] } as LengthBoundSpec;
const B_QR_SIZE   = { (millimeter) : [10.0, 30.0, 60.0] } as LengthBoundSpec;
const B_QR_FIT    = { (millimeter) : [0.0, 0.2, 1.0] } as LengthBoundSpec;
const B_QR_DEPTH  = { (millimeter) : [0.2, 0.4, 1.2] } as LengthBoundSpec;
const B_QR_R      = { (millimeter) : [0.5, 2.0, 6.0] } as LengthBoundSpec;
const B_QR_Z      = { (millimeter) : [10.0, 19.0, 200.0] } as LengthBoundSpec;
const B_LED_D     = { (millimeter) : [1.5, 3.1, 6.0] } as LengthBoundSpec;
const B_LED_S     = { (millimeter) : [4.0, 12.0, 30.0] } as LengthBoundSpec;
const B_LED_Z     = { (millimeter) : [5.0, 38.0, 200.0] } as LengthBoundSpec;
const B_EXPLODE   = { (millimeter) : [0.0, 0.0, 100.0] } as LengthBoundSpec;

// Las etiquetas "Name" de los parámetros solo admiten ASCII imprimible: sin tildes, ñ ni °
annotation { "Feature Type Name" : "Carcasa llaves ESP32",
             "Feature Type Description" : "Carcasa de pared con ranura para llavero y microswitch de rodillo con leva lateral." }
export const carcasaLlavesESP32 = defineFeature(function(context is Context, id is Id, definition is map)
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
        annotation { "Group Name" : "Tapa trasera y montaje", "Collapsed By Default" : true }
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
            annotation { "Name" : "Ojal de pared 1: altura" }
            isLength(definition.keyZ1, B_KEY_Z1);
            annotation { "Name" : "Ojal de pared 2: altura" }
            isLength(definition.keyZ2, B_KEY_Z2);
            annotation { "Name" : "Ojal: diametro para la cabeza" }
            isLength(definition.keyHead, B_KEY_HEAD);
            annotation { "Name" : "Ojal: ancho para la cana" }
            isLength(definition.keyShank, B_KEY_SHANK);
            annotation { "Name" : "Ojal: recorrido" }
            isLength(definition.keySlot, B_KEY_SLOT);
        }
        annotation { "Group Name" : "Cara frontal", "Collapsed By Default" : true }
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
            annotation { "Name" : "Agujeros para LED", "Default" : true }
            definition.leds is boolean;
            if (definition.leds)
            {
                annotation { "Name" : "LED: diametro del agujero" }
                isLength(definition.ledD, B_LED_D);
                annotation { "Name" : "LED: separacion entre centros" }
                isLength(definition.ledS, B_LED_S);
                annotation { "Name" : "LED: altura" }
                isLength(definition.ledZ, B_LED_Z);
            }
        }
        annotation { "Name" : "Separar tapa trasera (vista explosionada)" }
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
        const D1 = definition.connSpace + leverRest + COLLAR_T + definition.guideL + SLEEVE_T + slotW + wallF;
        const espFrontY = definition.pcbY + definition.pcbT + definition.headerH + definition.espFront;
        const D2 = espFrontY + 1 * millimeter + SLEEVE_T + slotW + wallF;
        const D = max(D1, D2);
        const ysF = D - wallF;                                               // cara interior de la pared frontal
        const ysB = ysF - slotW;                                             // cara trasera de la ranura
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
        if (abs(swX - cx) + definition.swL / 2 + 0.5 * millimeter > cx - wall)
            throw regenError("El microswitch no cabe a lo ancho: es demasiado largo o su rodillo está muy desplazado del centro.");
        if (definition.swScrews && definition.holeS / 2 + M2_CLEAR / 2 > definition.swL / 2)
            throw regenError("Los agujeros de montaje del microswitch quedan fuera de su cuerpo.");
        if (usbY - definition.usbOT / 2 < 0.5 * millimeter || usbY + definition.usbOT / 2 > D - wallF - 0.5 * millimeter)
            throw regenError("La abertura del USB no cabe en el fondo de la carcasa.");
        if (definition.cornerR > bIn &&
            sqrt(2) * (definition.cornerR - bIn) + max(definition.bossR, definition.cbD / 2 + 0.8 * millimeter) > definition.cornerR)
            throw regenError("Radio de esquinas demasiado grande: los tetones o los avellanados de la tapa atravesarían el canto redondeado.");
        if (definition.qrDepth > wallF - 0.8 * millimeter)
            throw regenError("El rebaje de la etiqueta QR es demasiado profundo para la pared frontal.");
        if (definition.qrSize / 2 + definition.qrFit > cx - definition.cornerR)
            throw regenError("La etiqueta QR es más ancha que la cara frontal.");
        if (definition.cbDepth > definition.backT - 0.8 * millimeter)
            throw regenError("El avellanado es demasiado profundo para el grosor de la tapa trasera.");
        if (zSlotBottom - SLEEVE_T < zB0)
            throw regenError("La ranura es demasiado profunda para la altura de la carcasa.");
        if (definition.qrZ - definition.qrSize / 2 - definition.qrFit < definition.chamferF + 2 * millimeter ||
            definition.qrZ + definition.qrSize / 2 + definition.qrFit > zSlotBottom - SLEEVE_T)
            throw regenError("La etiqueta QR no cabe a esa altura en la cara frontal.");
        if (definition.leds)
        {
            if (definition.ledZ + definition.ledD / 2 + 0.3 * millimeter > zSlotBottom - SLEEVE_T)
                throw regenError("Los LED chocan con el fondo de la ranura: bájalos.");
            if (definition.ledZ - definition.ledD / 2 < definition.qrZ + definition.qrSize / 2 + definition.qrFit + 0.5 * millimeter)
                throw regenError("Los LED se solapan con la etiqueta QR.");
        }
        for (var z in [definition.keyZ1, definition.keyZ2])
        {
            if (z - definition.keyHead / 2 < wall + 2 * millimeter || z + definition.keySlot + definition.keyHead / 2 > H - wall - 2 * millimeter)
                throw regenError("Un ojal de pared queda fuera de la tapa trasera.");
        }
        if (definition.backT < KEY_LEDGE + 0.6 * millimeter)
            throw regenError("Tapa trasera demasiado fina para los ojales.");
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

        // Camisa de la ranura (pegada a la pared frontal y al techo)
        caja(context, id + "inner" + "sleeve",
            vector(cx - sleeveHalf, ysB - SLEEVE_T, zSlotBottom - SLEEVE_T),
            vector(cx + sleeveHalf, ysF + EPS, H - wall + EPS));

        // Baldas del microswitch: el switch queda atrapado entre las dos (llegan a la pared frontal para
        // imprimirse sin voladizos). El empujador corre entre ellas.
        caja(context, id + "inner" + "shelf",
            vector(wall - EPS, yb, shelfTop - SHELF_T),
            vector(W - wall + EPS, ysF + EPS, shelfTop));
        caja(context, id + "inner" + "shelf2",
            vector(wall - EPS, yb, shelf2Bot),
            vector(W - wall + EPS, ysF + EPS, shelf2Bot + SHELF_T));

        // Guía del empujador, pegada a la camisa de la ranura (se apoya en ella al imprimir)
        caja(context, id + "inner" + "guide",
            vector(cx - definition.pushW / 2 - definition.fitGuide - GUIDE_T, yGuideB, zc - hP / 2 - definition.fitGuide - GUIDE_T),
            vector(cx + definition.pushW / 2 + definition.fitGuide + GUIDE_T, ysB - SLEEVE_T + EPS, zc + hP / 2 + definition.fitGuide + GUIDE_T));

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

        // ====================== 5. CARCASA: VACIADOS ======================
        // Ranura del llavero (atraviesa el techo)
        caja(context, id + "cuts" + "slot",
            vector(cx - slotHalf, ysB, zSlotBottom),
            vector(cx + slotHalf, ysF, H + 1 * millimeter));
        // Agujero de la guía del empujador: atraviesa la guía y la camisa hasta la ranura
        // (su alto coincide con el hueco entre baldas, así el empujador entra por detrás)
        caja(context, id + "cuts" + "guideHole",
            vector(cx - definition.pushW / 2 - definition.fitGuide, yGuideB - 1 * millimeter, zc - hP / 2 - definition.fitGuide),
            vector(cx + definition.pushW / 2 + definition.fitGuide, ysB + 0.05 * millimeter, zc + hP / 2 + definition.fitGuide));
        // Agujeros para insertos térmicos
        for (var i = 0; i < 4; i += 1)
        {
            cylY(context, id + "cuts" + ("insert" ~ toString(i)), bossPts[i][0], bossPts[i][1], -1 * millimeter, definition.insertL, definition.insertD / 2);
        }
        // Microswitch: paso M2 en la balda inferior + piloto pasante en la superior (M2 x 10 desde abajo;
        // la punta sale al hueco vacío de encima). Montar antes que la protoboard.
        if (definition.swScrews)
        {
            for (var s in [-1, 1])
            {
                const hx = swX + s * definition.holeS / 2;
                const hy = yb + definition.holeY;
                cylZ(context, id + "cuts" + ("swHole" ~ toString(s + 1)), hx, hy,
                     shelfTop - SHELF_T - 1 * millimeter, shelfTop + 0.1 * millimeter, M2_CLEAR / 2);
                cylZ(context, id + "cuts" + ("swPilot" ~ toString(s + 1)), hx, hy,
                     shelf2Bot - 0.1 * millimeter, shelf2Bot + SHELF_T + 1 * millimeter, M2_PILOT / 2);
            }
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
        // Rebaje de la etiqueta QR en la cara frontal
        const qrHalf = definition.qrSize / 2 + definition.qrFit;
        caja(context, id + "cuts" + "qr",
            vector(cx - qrHalf, D - definition.qrDepth, definition.qrZ - qrHalf),
            vector(cx + qrHalf, D + 1 * millimeter, definition.qrZ + qrHalf));
        opFillet(context, id + "cuts" + "qrFillet", {
                    "entities" : qParallelEdges(qCreatedBy(id + "cuts" + "qr", EntityType.EDGE), Y_DIRECTION),
                    "radius" : min(definition.qrR, qrHalf - 0.1 * millimeter) });
        // Agujeros para LED
        if (definition.leds)
        {
            for (var s in [-1, 1])
            {
                cylY(context, id + "cuts" + ("led" ~ toString(s + 1)), cx + s * definition.ledS / 2, definition.ledZ,
                     ysF - 0.5 * millimeter, D + 1 * millimeter, definition.ledD / 2);
            }
        }
        subtract(context, id + "doCuts", bodyAt(id, bodyPoint), qCreatedBy(id + "cuts", EntityType.BODY));

        // Chaflán de entrada de la ranura
        const bodyEdges = qOwnedByBody(bodyAt(id, bodyPoint), EntityType.EDGE);
        const midY = (ysB + ysF) / 2;
        // "try silent": si el chaflán falla solo queda el aviso propio, sin el error interno en la lista de avisos
        try silent
        {
            opChamfer(context, id + "slotChamfer", {
                        "entities" : qUnion([qContainsPoint(bodyEdges, vector(cx, ysB, H)),
                                             qContainsPoint(bodyEdges, vector(cx, ysF, H)),
                                             qContainsPoint(bodyEdges, vector(cx - slotHalf, midY, H)),
                                             qContainsPoint(bodyEdges, vector(cx + slotHalf, midY, H))]),
                        "chamferType" : ChamferType.EQUAL_OFFSETS,
                        "width" : SLOT_CHAMFER });
        }
        catch
        {
            reportFeatureWarning(context, id, "No se pudo achaflanar la entrada de la ranura; el resto del modelo es válido.");
        }

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
            cylY(context, id + "backCuts" + ("screw" ~ toString(i)), bossPts[i][0], bossPts[i][1], yBack - 1 * millimeter, 1 * millimeter, definition.screwD / 2);
            if (definition.cbDepth > 0.01 * millimeter)
            {
                cylY(context, id + "backCuts" + ("cb" ~ toString(i)), bossPts[i][0], bossPts[i][1], yBack - 1 * millimeter, yBack + definition.cbDepth, definition.cbD / 2);
            }
        }
        // Ojales para colgar (el tornillo entra por el círculo y la caja baja hasta el final de la ranura)
        var k = 0;
        for (var z in [definition.keyZ1, definition.keyZ2])
        {
            const kid = toString(k);
            cylY(context, id + "backCuts" + ("keyEntry" ~ kid), cx, z, yBack - 1 * millimeter, 1 * millimeter, definition.keyHead / 2);
            caja(context, id + "backCuts" + ("keySlot" ~ kid),
                vector(cx - definition.keyShank / 2, yBack - 1 * millimeter, z),
                vector(cx + definition.keyShank / 2, 1 * millimeter, z + definition.keySlot));
            cylY(context, id + "backCuts" + ("keySlotEnd" ~ kid), cx, z + definition.keySlot, yBack - 1 * millimeter, 1 * millimeter, definition.keyShank / 2);
            caja(context, id + "backCuts" + ("keyCham" ~ kid),
                vector(cx - definition.keyHead / 2, yBack + KEY_LEDGE, z),
                vector(cx + definition.keyHead / 2, 1 * millimeter, z + definition.keySlot));
            cylY(context, id + "backCuts" + ("keyChamEnd" ~ kid), cx, z + definition.keySlot, yBack + KEY_LEDGE, 1 * millimeter, definition.keyHead / 2);
            k += 1;
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
        //  0 "Prueba mecanismo":           ranura + camisa + guía + baldas del switch (con el Empujador real)
        //  1 "Prueba esquina inserto M3":  esquina inferior izquierda de la carcasa, últimos mm del tetón
        //  2 "Prueba paso USB":            trozo de la base con la abertura de la clavija
        //  3 "Prueba tapa y protoboard":   marco de la tapa con las 4 escuadras, soportes y avellanados inferiores
        var massInfo = "";
        if (definition.testMode)
        {
            const xm = sleeveHalf + 1 * millimeter;
            const zm0 = zSlotBottom - SLEEVE_T - 1 * millimeter;
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
            // Ventana del marco: deja las escuadras y engloba los ojales enteros (si los cortara dejaría tiras finísimas)
            const xw = definition.pcbW / 2 - FENCE_IN;
            const keyLo = min(definition.keyZ1, definition.keyZ2) - definition.keyHead / 2;
            const keyHi = max(definition.keyZ1, definition.keyZ2) + definition.keySlot + definition.keyHead / 2;
            const zw0 = min(zB0 + FENCE_IN, keyLo - 1 * millimeter);
            const zw1 = max(zB1 - FENCE_IN, keyHi + 1 * millimeter);
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
            const couponPts = [vector(cx + slotHalf + SLEEVE_T / 2, (ysB + ysF) / 2, zSlotBottom + 3 * millimeter),
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
                        "entities" : qSubtraction(qUnion([bodyAt(id, bodyPoint), bodyAt(id, backPoint)]), qCreatedBy(id + "test", EntityType.BODY)) });

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
            const names = ["Prueba mecanismo", "Prueba esquina inserto M3", "Prueba paso USB", "Prueba tapa y protoboard"];
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
            const grams = evVolume(context, { "entities" : qUnion([qCreatedBy(id + "test", EntityType.BODY), bodyAt(id, pushPoint)]) }) /
                          (centimeter ^ 3) * PETG_DENSITY;
            massInfo = "PIEZAS DE PRUEBA (" ~ toString(roundToPrecision(grams, 0)) ~ " g de PETG como mucho, al 100 % de relleno) · ";
        }

        // ====================== 9. NOMBRES, EXPLOSIÓN E INFORME ======================
        if (!definition.testMode)
        {
            setProperty(context, { "entities" : bodyAt(id, bodyPoint), "propertyType" : PropertyType.NAME, "value" : "Carcasa frontal" });
            setProperty(context, { "entities" : bodyAt(id, backPoint), "propertyType" : PropertyType.NAME, "value" : "Tapa trasera" });
        }
        setProperty(context, { "entities" : bodyAt(id, pushPoint), "propertyType" : PropertyType.NAME, "value" : "Empujador" });
        if (definition.explode > 0.01 * millimeter)
        {
            opTransform(context, id + "explode", {
                        "bodies" : bodyAt(id, backPoint),
                        "transform" : transform(vector(ZERO, -definition.explode, ZERO)) });
        }
        reportFeatureInfo(context, id, massInfo ~ "Exterior (ancho x fondo x alto): " ~ toString(roundToPrecision(W / millimeter, 1)) ~ " x " ~
                    toString(roundToPrecision((D + definition.backT) / millimeter, 1)) ~ " x " ~ toString(roundToPrecision(H / millimeter, 1)) ~
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
        "keyZ1" : 22.0 * millimeter, "keyZ2" : 52.0 * millimeter, "keyHead" : 8.5 * millimeter, "keyShank" : 4.2 * millimeter,
        "keySlot" : 8.0 * millimeter,
        "qrSize" : 30.0 * millimeter, "qrFit" : 0.2 * millimeter, "qrDepth" : 0.4 * millimeter, "qrR" : 2.0 * millimeter,
        "qrZ" : 19.0 * millimeter,
        "leds" : true, "ledD" : 3.1 * millimeter, "ledS" : 12.0 * millimeter, "ledZ" : 38.0 * millimeter,
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
