from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.colors import HexColor
from reportlab.platypus import (
    SimpleDocTemplate,
    Paragraph,
    Spacer,
    ListFlowable,
    ListItem,
    Table,
    TableStyle,
    HRFlowable,
    PageBreak,
)

out = r"C:\Users\musharp\OneDrive\Desktop\TaskFlow\SignalR_Notification_Errors_Interview_Guide.pdf"

doc = SimpleDocTemplate(
    out,
    pagesize=A4,
    leftMargin=0.7 * inch,
    rightMargin=0.7 * inch,
    topMargin=0.65 * inch,
    bottomMargin=0.65 * inch,
)

primary = HexColor("#4F46E5")
dark = HexColor("#0F172A")
muted = HexColor("#475569")
soft = HexColor("#EEF2FF")

styles = getSampleStyleSheet()
styles.add(
    ParagraphStyle(
        name="CoverTitle",
        fontName="Helvetica-Bold",
        fontSize=22,
        textColor=dark,
        leading=28,
        spaceAfter=8,
    )
)
styles.add(
    ParagraphStyle(
        name="CoverSub",
        fontName="Helvetica",
        fontSize=11,
        textColor=muted,
        leading=16,
        spaceAfter=18,
    )
)
styles.add(
    ParagraphStyle(
        name="H1",
        fontName="Helvetica-Bold",
        fontSize=14,
        textColor=primary,
        leading=18,
        spaceBefore=14,
        spaceAfter=8,
    )
)
styles.add(
    ParagraphStyle(
        name="H2",
        fontName="Helvetica-Bold",
        fontSize=11.5,
        textColor=dark,
        leading=15,
        spaceBefore=10,
        spaceAfter=5,
    )
)
styles.add(
    ParagraphStyle(
        name="Body",
        fontName="Helvetica",
        fontSize=10,
        textColor=dark,
        leading=14,
        spaceAfter=6,
    )
)
styles.add(
    ParagraphStyle(
        name="BulletBody",
        fontName="Helvetica",
        fontSize=10,
        textColor=dark,
        leading=13,
    )
)
styles.add(
    ParagraphStyle(
        name="CodeBlock",
        fontName="Courier",
        fontSize=8.5,
        textColor=dark,
        leading=12,
        backColor=soft,
        leftIndent=4,
        rightIndent=4,
        spaceBefore=4,
        spaceAfter=8,
    )
)
styles.add(
    ParagraphStyle(
        name="Q",
        fontName="Helvetica-Bold",
        fontSize=10,
        textColor=dark,
        leading=13,
        spaceBefore=8,
        spaceAfter=3,
    )
)
styles.add(
    ParagraphStyle(
        name="A",
        fontName="Helvetica",
        fontSize=10,
        textColor=muted,
        leading=13,
        spaceAfter=4,
    )
)
styles.add(
    ParagraphStyle(
        name="FooterNote",
        fontName="Helvetica-Oblique",
        fontSize=8.5,
        textColor=muted,
        leading=11,
    )
)
styles.add(
    ParagraphStyle(
        name="Tag",
        fontName="Helvetica-Bold",
        fontSize=9,
        textColor=primary,
        leading=12,
        spaceAfter=10,
    )
)

story = []

story.append(Paragraph("Interview Guide", styles["Tag"]))
story.append(
    Paragraph(
        "SignalR + Notification Errors<br/>How We Debugged &amp; Fixed Them",
        styles["CoverTitle"],
    )
)
story.append(
    Paragraph(
        "TaskFlow (ASP.NET Core + Next.js + RTK Query + SignalR)<br/>"
        "Real production-style debugging story for interviews — problem, root cause, fix, and talking points.",
        styles["CoverSub"],
    )
)

story.append(HRFlowable(width="100%", thickness=1, color=primary, spaceAfter=12))

story.append(
    Paragraph("1. Problem Statement (How to explain in interview)", styles["H1"])
)
story.append(
    Paragraph(
        "After adding real-time task-assignment notifications with SignalR, the UI started throwing Next.js error overlays "
        "when opening the notification bell or clicking a notification. It looked like a framework crash, but the app "
        "was mostly fine — the hard part was separating <b>real bugs</b> from <b>dev-only noise</b>.",
        styles["Body"],
    )
)

story.append(Paragraph("Errors we saw in console / overlay:", styles["H2"]))
story.append(
    ListFlowable(
        [
            ListItem(
                Paragraph(
                    "<b>Error A:</b> <font face='Courier' size='9'>Failed to start the connection: The connection was stopped during negotiation</font> (SignalR)",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "<b>Error B:</b> <font face='Courier' size='9'>Cannot refetch a query that has not been started yet</font> (RTK Query) at <font face='Courier' size='9'>NotificationBell.tsx</font>",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "<b>Error C (noise):</b> Hydration mismatch with <font face='Courier' size='9'>cz-shortcut-listen</font> — browser extension, not our app",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
        ],
        bulletType="bullet",
        start="•",
    )
)

story.append(Paragraph("2. Why this felt hard", styles["H1"]))
story.append(
    Paragraph(
        "Three layers failed at once: real-time transport (SignalR), client data layer (RTK Query), and Next.js Dev overlay "
        "(which turns <font face='Courier' size='9'>console.error</font> into a full-screen error). "
        "If you only chase SignalR, you miss the RTK bug. If you only silence logs, the real refetch crash remains.",
        styles["Body"],
    )
)

story.append(Paragraph("3. Debugging approach (interview-friendly steps)", styles["H1"]))
story.append(
    ListFlowable(
        [
            ListItem(
                Paragraph(
                    "<b>Reproduce with evidence:</b> open DevTools → Console + Network. Note exact stack frames (<font face='Courier' size='9'>NotificationBell.tsx</font>, SignalR negotiate).",
                    styles["BulletBody"],
                ),
                leftIndent=10,
            ),
            ListItem(
                Paragraph(
                    "<b>Verify backend independently:</b> login API OK, hub negotiate returns 401 without token / works with JWT → hub itself is alive.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
            ),
            ListItem(
                Paragraph(
                    "<b>Split errors:</b> one stack from RTK Query, one from SignalR logger. Fix deterministic app bugs first.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
            ),
            ListItem(
                Paragraph(
                    "<b>Check React Strict Mode:</b> in Next.js/React 18+, effects mount → cleanup → remount in dev. That aborts in-flight SignalR negotiate.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
            ),
            ListItem(
                Paragraph(
                    "<b>Ignore false positives:</b> hydration attrs injected by extensions are not product bugs.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
            ),
        ],
        bulletType="1",
        start="1",
    )
)

story.append(Paragraph("4. Root Cause Deep Dive", styles["H1"]))

story.append(Paragraph("4.1 RTK Query: refetch on a skipped query", styles["H2"]))
story.append(
    Paragraph(
        "Notification list query used <font face='Courier' size='9'>skip: !open</font>. On bell click we called "
        "<font face='Courier' size='9'>refetchList()</font> <b>before</b> the query was allowed to start. "
        "RTK throws: <i>Cannot refetch a query that has not been started yet</i>. This was a real uncaught UI bug.",
        styles["Body"],
    )
)
story.append(Paragraph("Bad pattern:", styles["Body"]))
story.append(
    Paragraph(
        "setOpen(true); refetchList(); // query still skipped / not started",
        styles["CodeBlock"],
    )
)
story.append(Paragraph("Fix idea:", styles["Body"]))
story.append(
    Paragraph(
        "Just setOpen(true). When skip becomes false, RTK auto-fetches. Never refetch a skipped query.",
        styles["CodeBlock"],
    )
)

story.append(Paragraph("4.2 SignalR: stopped during negotiation", styles["H2"]))
story.append(
    Paragraph(
        "In development, React Strict Mode runs effects twice. Flow: <b>start()</b> begins negotiate → cleanup calls "
        "<b>stop()</b> → negotiate aborts → client reports <i>connection was stopped during negotiation</i>. "
        "SignalR JS logs this with <font face='Courier' size='9'>console.error</font>, and Next.js DevTools overlays it as a scary Console Error — even if we catch the promise.",
        styles["Body"],
    )
)

story.append(Paragraph("4.3 Next.js overlay amplifies noise", styles["H2"]))
story.append(
    Paragraph(
        "Next.js intercepts console errors in development. Library-level SignalR logging looked like an application crash. "
        "Solution: reduce SignalR log level / use a shared connection so Strict Mode remounts do not tear down negotiation mid-flight.",
        styles["Body"],
    )
)

story.append(PageBreak())
story.append(Paragraph("5. Final Solution Architecture", styles["H1"]))

story.append(Paragraph("A) NotificationBell (RTK)", styles["H2"]))
story.append(
    ListFlowable(
        [
            ListItem(
                Paragraph(
                    "Keep unread-count query always active (badge).",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "Keep notifications list query <font face='Courier' size='9'>skip: !open</font>.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "On toggle: only flip <font face='Courier' size='9'>open</font> — no manual refetch of skipped query.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
        ],
        bulletType="bullet",
        start="•",
    )
)

story.append(Paragraph("B) Shared SignalR singleton", styles["H2"]))
story.append(
    ListFlowable(
        [
            ListItem(
                Paragraph(
                    "One module-level HubConnection (not created/destroyed on every React effect).",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "Subscribers register handlers; Strict Mode remount only adds/removes handlers.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "<font face='Courier' size='9'>configureLogging(LogLevel.None)</font> to avoid Next overlay spam.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "Treat negotiate-abort messages as benign in catch.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
        ],
        bulletType="bullet",
        start="•",
    )
)

story.append(Paragraph("C) Backend SignalR (ASP.NET Core)", styles["H2"]))
story.append(
    ListFlowable(
        [
            ListItem(
                Paragraph(
                    "Hub: <font face='Courier' size='9'>/hubs/notifications</font> with JWT via <font face='Courier' size='9'>access_token</font> query (WebSockets).",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "On connect: join group <font face='Courier' size='9'>user-{userId}</font>.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "On task assign: save <font face='Courier' size='9'>Notifications</font> row, then push <font face='Courier' size='9'>ReceiveNotification</font> to that group.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
            ListItem(
                Paragraph(
                    "CORS: frontend origin + credentials for SignalR negotiate.",
                    styles["BulletBody"],
                ),
                leftIndent=10,
                value="•",
            ),
        ],
        bulletType="bullet",
        start="•",
    )
)

data = [
    [
        Paragraph("<b>Layer</b>", styles["BulletBody"]),
        Paragraph("<b>Symptom</b>", styles["BulletBody"]),
        Paragraph("<b>Fix</b>", styles["BulletBody"]),
    ],
    [
        Paragraph("RTK Query", styles["BulletBody"]),
        Paragraph("refetch before start", styles["BulletBody"]),
        Paragraph("toggle open only; let skip unlock fetch", styles["BulletBody"]),
    ],
    [
        Paragraph("SignalR + React", styles["BulletBody"]),
        Paragraph("stopped during negotiation", styles["BulletBody"]),
        Paragraph(
            "shared connection + no Strict Mode tear-down", styles["BulletBody"]
        ),
    ],
    [
        Paragraph("Next.js Dev", styles["BulletBody"]),
        Paragraph("full-screen Console Error", styles["BulletBody"]),
        Paragraph("LogLevel.None / ignore benign aborts", styles["BulletBody"]),
    ],
    [
        Paragraph("Hydration", styles["BulletBody"]),
        Paragraph("cz-shortcut-listen", styles["BulletBody"]),
        Paragraph(
            "extension noise; suppressHydrationWarning on body", styles["BulletBody"]
        ),
    ],
]
table = Table(data, colWidths=[1.35 * inch, 2.35 * inch, 2.9 * inch])
table.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, 0), soft),
            ("TEXTCOLOR", (0, 0), (-1, -1), dark),
            ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
            ("FONTSIZE", (0, 0), (-1, -1), 9),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("GRID", (0, 0), (-1, -1), 0.5, HexColor("#C7D2FE")),
            ("LEFTPADDING", (0, 0), (-1, -1), 6),
            ("RIGHTPADDING", (0, 0), (-1, -1), 6),
            ("TOPPADDING", (0, 0), (-1, -1), 6),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
        ]
    )
)
story.append(Spacer(1, 8))
story.append(table)

story.append(Paragraph("6. Interview Q&amp;A (memorize these answers)", styles["H1"]))

qa = [
    (
        "Q1. What was the actual bug vs the scary overlay?",
        "The real uncaught bug was RTK refetch on a skipped query. The SignalR negotiate message was largely a Strict Mode race amplified by Next.js treating library console.error as an overlay.",
    ),
    (
        "Q2. Why does React Strict Mode break SignalR start?",
        "Effects run mount → cleanup → remount in development. If start() is still negotiating when cleanup calls stop(), the client throws 'stopped during negotiation'.",
    ),
    (
        "Q3. How do you authenticate SignalR with JWT?",
        "Browsers cannot easily set Authorization on WebSocket upgrades, so ASP.NET reads access_token from the query string for /hubs paths via JwtBearer OnMessageReceived.",
    ),
    (
        "Q4. How do you push to one user only?",
        "On connect, add the connection to group user-{id}. On assign, SendAsync to that group with the notification DTO.",
    ),
    (
        "Q5. How would you prevent duplicate SignalR connections?",
        "Use a module singleton (or context provider) with subscribers, instead of creating a new HubConnection in every component effect.",
    ),
    (
        "Q6. What would you improve further in production?",
        "Reconnect metrics, presence, mark-as-delivered, integration tests for assign→notify, and polling fallback if WebSockets are blocked.",
    ),
]
for q, a in qa:
    story.append(Paragraph(q, styles["Q"]))
    story.append(Paragraph(a, styles["A"]))

story.append(Paragraph("7. One-minute interview pitch (say this)", styles["H1"]))
story.append(
    Paragraph(
        "\"We added SignalR notifications for task assignment. In Next.js dev we hit two issues: RTK Query refetch on a skipped "
        "notification list query, and SignalR negotiate abort under React Strict Mode which Next displayed as a console error overlay. "
        "I isolated each stack in DevTools, validated the hub with authenticated negotiate, fixed the RTK toggle logic, and moved "
        "SignalR to a shared connection with quiet logging. Backend still persists Notifications and pushes ReceiveNotification to "
        "user-specific groups over JWT-authenticated hubs.\"",
        styles["Body"],
    )
)

story.append(Spacer(1, 10))
story.append(
    HRFlowable(width="100%", thickness=1, color=primary, spaceBefore=4, spaceAfter=8)
)
story.append(
    Paragraph(
        "TaskFlow • Interview prep notes • SignalR + RTK Query + Next.js debugging",
        styles["FooterNote"],
    )
)

doc.build(story)
print(out)
