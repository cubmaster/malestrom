# Deploy (Docker / Kubernetes)

Backend services for Iron Exiles are implemented as **containers** per `.adlc/ETHOS.md` §7:

- **Local development:** Docker Compose (see REQ-042 Auth stack)
- **Production:** Kubernetes manifests under `deploy/k8s/` (added in REQ-042+)

The **Unity game client** is not containerized for player installs. **Dedicated game servers** (headless Unity builds) and backend microservices are.

Legacy Unreal dedicated-server notes in older specs are superseded by ADR-034.
